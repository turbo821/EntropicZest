using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;
using System.Threading.Tasks;

namespace EntropicZest
{
    public partial class MainForm : Form
    {
        private List<double> data = new List<double>();
        private double binSize;
        private CancellationTokenSource cts;
        public MainForm()
        {
            InitializeComponent();
            ExcelPackage.License.SetNonCommercialPersonal("Hello");
            progressBar.Visible = false;
            btnCancel.Visible = false;
        }

        private async void btnLoadExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx;*.xls",
                Title = "Выберите Excel файл"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SetBusyState(true);
                    string filePath = openFileDialog.FileName;
                    await LoadDataFromExcelAsync(filePath);
                    MessageBox.Show("Данные успешно загружены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (OperationCanceledException)
                {
                    MessageBox.Show("Операция отменена пользователем.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusyState(false);
                }
            }
        }

        private async Task LoadDataFromExcelAsync(string filePath)
        {
            cts = new CancellationTokenSource();
            data.Clear();

            IProgress<int> progress = new Progress<int>(percent =>
            {
                progressBar.Value = percent;
                lblStatus.Text = $"Загрузка данных... {percent}%";
            });

            await Task.Run(() =>
            {
                FileInfo fileInfo = new FileInfo(filePath);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns + 1;
                    int totalCells = (colCount - 1) * rowCount;
                    int processedCells = 0;

                    for (int col = 2; col <= colCount; col++)
                    {
                        for (int row = 1; row <= rowCount; row++)
                        {
                            cts.Token.ThrowIfCancellationRequested();

                            var cellValue = worksheet.Cells[row, col].Text;
                            if (double.TryParse(cellValue, out double value))
                            {
                                data.Add(value);
                            }

                            processedCells++;
                            int percent = (int)((double)processedCells / totalCells * 100);
                            progress.Report(percent);
                        }
                    }
                }
            }, cts.Token);
        }

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("Сначала загрузите данные из Excel!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!double.TryParse(txtBinSize.Text, out binSize) || binSize <= 0)
            {
                MessageBox.Show("Введите корректный размер бина!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                SetBusyState(true);
                await Task.Run(() => CalculateAndDisplayResults());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при расчете: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusyState(false);
            }
        }

        private void CalculateAndDisplayResults()
        {
            double minValue = data.Min();
            double maxValue = data.Max();
            int intervalCount = (int)Math.Ceiling((maxValue - minValue) / binSize);

            Dictionary<(double, double), int> distribution = new Dictionary<(double, double), int>();

            for (int i = 0; i < intervalCount; i++)
            {
                double start = minValue + i * binSize;
                double end = Math.Min(start + binSize, maxValue);
                distribution[(start, end)] = 0;
            }

            foreach (var value in data)
            {
                foreach (var interval in distribution.Keys)
                {
                    if (value >= interval.Item1 && value < interval.Item2)
                    {
                        distribution[interval]++;
                        break;
                    }
                }
            }

            double entropy = CalculateEntropy(distribution, data.Count);

            this.Invoke((MethodInvoker)delegate
            {
                txtStatistics.Clear();
                lstDistribution.Clear();

                txtStatistics.AppendText($"Минимальное значение: {minValue}\n");
                txtStatistics.AppendText($"Максимальное значение: {maxValue}\n");
                txtStatistics.AppendText($"Количество точек: {data.Count}\n");
                txtStatistics.AppendText($"Размер бина: {binSize}\n");
                txtStatistics.AppendText($"Количество интервалов: {intervalCount}\n");
                txtStatistics.AppendText($"Энтропия Шеннона: {entropy:F4}\n\n");
                lstDistribution.AppendText("Распределение по интервалам:\n");

                foreach (var interval in distribution)
                {
                    lstDistribution.AppendText($"[{interval.Key.Item1:F2}, {interval.Key.Item2:F2}]: {interval.Value} точек\n");
                }
            });
        }

        private double CalculateEntropy(Dictionary<(double, double), int> distribution, int totalCount)
        {
            double entropy = 0;

            foreach (var count in distribution.Values)
            {
                if (count > 0)
                {
                    double probability = (double)count / totalCount;
                    entropy -= probability * Math.Log(probability, 2);
                }
            }

            return entropy;
        }

        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Сохранить результаты"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                SaveResultsToExcel(filePath);
                MessageBox.Show("Результаты успешно сохранены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveResultsToExcel(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Результаты");

                // Записываем статистические данные
                worksheet.Cells[1, 1].Value = "Минимальное значение";
                worksheet.Cells[1, 2].Value = double.Parse(txtStatistics.Lines[0].Split(':')[1]);

                worksheet.Cells[2, 1].Value = "Максимальное значение";
                worksheet.Cells[2, 2].Value = double.Parse(txtStatistics.Lines[1].Split(':')[1]);

                worksheet.Cells[3, 1].Value = "Количество точек";
                worksheet.Cells[3, 2].Value = int.Parse(txtStatistics.Lines[2].Split(':')[1]);

                worksheet.Cells[4, 1].Value = "Размер бина";
                worksheet.Cells[4, 2].Value = double.Parse(txtStatistics.Lines[3].Split(':')[1]);

                worksheet.Cells[5, 1].Value = "Количество интервалов";
                worksheet.Cells[5, 2].Value = int.Parse(txtStatistics.Lines[4].Split(':')[1]);

                worksheet.Cells[6, 1].Value = "Энтропия Шеннона";
                worksheet.Cells[6, 2].Value = double.Parse(txtStatistics.Lines[5].Split(':')[1]);

                // Записываем распределение по интервалам
                worksheet.Cells[8, 1].Value = "Начало интервала";
                worksheet.Cells[8, 2].Value = "Конец интервала";
                worksheet.Cells[8, 3].Value = "Количество точек";

                int rowIndex = 9;
                foreach (var line in lstDistribution.Lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Разделяем строку на интервал и количество точек
                    var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length == 2) // Проверяем, что строка содержит две части
                    {
                        // Обрабатываем интервал
                        string intervalPart = parts[0].Trim(); // [-11.73, -11.23]
                        string interval = intervalPart.Trim('[', ']'); // Убираем квадратные скобки

                        // Разделяем интервал на начало и конец
                        var intervalBounds = interval.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (intervalBounds.Length == 2 &&
                            double.TryParse(intervalBounds[0].Trim(), out double start) &&
                            double.TryParse(intervalBounds[1].Trim(), out double end))
                        {
                            // Обрабатываем количество точек
                            string pointCountPart = parts[1].Trim(); // "1 точек"
                            int pointCount = int.Parse(pointCountPart.Split(' ')[0]); // Берем первое число

                            // Записываем данные в Excel
                            worksheet.Cells[rowIndex, 1].Value = start; // Начало интервала
                            worksheet.Cells[rowIndex, 2].Value = end;   // Конец интервала
                            worksheet.Cells[rowIndex, 3].Value = pointCount; // Количество точек
                            rowIndex++;
                        }
                    }
                }

                package.Save();
            }
        }

        private void btnCompareResults_Click(object sender, EventArgs e)
        {
            // Диалоговое окно для выбора первого файла
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Выберите первый файл для сравнения"
            };

            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Первый файл не выбран!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath1 = openFileDialog1.FileName;

            // Диалоговое окно для выбора второго файла
            OpenFileDialog openFileDialog2 = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Выберите второй файл для сравнения"
            };

            if (openFileDialog2.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Второй файл не выбран!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath2 = openFileDialog2.FileName;

            try
            {
                // Чтение энтропии из первого файла
                double entropy1 = ReadEntropyFromExcel(filePath1);

                // Чтение энтропии из второго файла
                double entropy2 = ReadEntropyFromExcel(filePath2);

                // Вычисление разности энтропий
                double entropyDifference = entropy1 - entropy2;

                // Формирование сообщения о разности энтропий и сравнении
                string resultMessage = $"Разность энтропий: {entropyDifference:F4}\n";

                if (entropy1 > entropy2)
                {
                    resultMessage += "Первые результаты имеют большую энтропию.";
                }
                else if (entropy1 < entropy2)
                {
                    resultMessage += "Вторые результаты имеют большую энтропию.";
                }
                else
                {
                    resultMessage += "Энтропии результатов равны.";
                }

                // Вывод результата в TextBox
                txtEntropyDifference.Clear();
                txtEntropyDifference.AppendText(resultMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сравнении файлов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private double ReadEntropyFromExcel(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Первый лист

                // Считываем значение энтропии из ячейки
                double entropy = worksheet.Cells[6, 2].GetValue<double>();

                return entropy;
            }
        }

        private void btnLoadResults_Click(object sender, EventArgs e)
        {
            // Диалоговое окно для выбора файла
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                Title = "Выберите файл с результатами"
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Файл не выбран!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string filePath = openFileDialog.FileName;

            try
            {
                // Очищаем текущие данные
                txtStatistics.Clear();
                lstDistribution.Clear();

                // Загружаем данные из файла
                LoadResultsFromExcel(filePath);

                MessageBox.Show("Результаты успешно загружены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке результатов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadResultsFromExcel(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Первый лист

                // Читаем статистические данные
                double minValue = worksheet.Cells[1, 2].GetValue<double>();
                double maxValue = worksheet.Cells[2, 2].GetValue<double>();
                int pointCount = worksheet.Cells[3, 2].GetValue<int>();
                double binSize = worksheet.Cells[4, 2].GetValue<double>();
                int intervalCount = worksheet.Cells[5, 2].GetValue<int>();
                double entropy = worksheet.Cells[6, 2].GetValue<double>();

                // Отображаем статистические данные в txtStatistics
                txtStatistics.AppendText($"Минимальное значение: {minValue}\n");
                txtStatistics.AppendText($"Максимальное значение: {maxValue}\n");
                txtStatistics.AppendText($"Количество точек: {pointCount}\n");
                txtStatistics.AppendText($"Размер бина: {binSize}\n");
                txtStatistics.AppendText($"Количество интервалов: {intervalCount}\n");
                txtStatistics.AppendText($"Энтропия Шеннона: {entropy:F4}\n\n");

                // Читаем распределение по интервалам
                int rowIndex = 9; // Начинаем с 9 строки (где начинаются интервалы)
                while (!string.IsNullOrWhiteSpace(worksheet.Cells[rowIndex, 1].Text) &&
                       !string.IsNullOrWhiteSpace(worksheet.Cells[rowIndex, 2].Text))
                {
                    double intervalStart = worksheet.Cells[rowIndex, 1].GetValue<double>();
                    double intervalEnd = worksheet.Cells[rowIndex, 2].GetValue<double>();
                    int pointCountInInterval = worksheet.Cells[rowIndex, 3].GetValue<int>();

                    // Формируем строку интервала
                    string interval = $"[{intervalStart:F2}, {intervalEnd:F2}]";

                    // Добавляем интервал в lstDistribution
                    lstDistribution.AppendText($"{interval}: {pointCountInInterval} точек\n");
                    rowIndex++;
                }
            }
        }

        private void btnBuildGraph_Click(object sender, EventArgs e)
        {
            if (lstDistribution.Lines.Count() == 0)
            {
                MessageBox.Show("Нет данных для построения графика!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BuildGraph();
        }

        private void BuildGraph()
        {
            // Очищаем предыдущие данные графика
            chartDistribution.Series.Clear();

            // Создаем новую серию данных
            Series series = new Series("Распределение точек")
            {
                ChartType = SeriesChartType.Column, // Гистограмма
                IsValueShownAsLabel = true // Показывать значения над столбцами
            };

            // Заполняем данные из lstDistribution.Lines
            foreach (var line in lstDistribution.Lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Разделяем строку на интервал и количество точек
                var parts = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2) // Проверяем, что строка содержит две части
                {
                    // Обрабатываем интервал
                    string intervalPart = parts[0].Trim(); // [-11.73, -11.23]
                    string interval = intervalPart.Trim('[', ']'); // Убираем квадратные скобки

                    // Разделяем интервал на начало и конец
                    var intervalBounds = interval.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (intervalBounds.Length == 2 &&
                        double.TryParse(intervalBounds[0].Trim(), out double start) &&
                        double.TryParse(intervalBounds[1].Trim(), out double end))
                    {
                        // Обрабатываем количество точек
                        string pointCountPart = parts[1].Trim(); // "1 точек"
                        int pointCountt = int.Parse(pointCountPart.Split(' ')[0]); // Берем первое число

                        // Добавляем данные в серию
                        string intervalLabel = $"[{start:F2}, {end:F2}]";
                        series.Points.AddXY(intervalLabel, pointCountt);
                    }
                }
            }

            // Добавляем серию на график
            chartDistribution.Series.Add(series);

            // Настройка осей
            chartDistribution.ChartAreas[0].AxisX.Title = "Интервалы";
            chartDistribution.ChartAreas[0].AxisY.Title = "Количество точек";
            chartDistribution.ChartAreas[0].AxisX.Interval = 1; // Отображать все метки на оси X

            int pointCount = series.Points.Count;
            chartDistribution.Width = Math.Max(panelGraph.Width, pointCount * 30); // Ширина зависит от количества точек

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cts?.Cancel();
        }

        private void SetBusyState(bool isBusy)
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnLoadExcel.Enabled = !isBusy;
                btnCalculate.Enabled = !isBusy;
                btnSaveResults.Enabled = !isBusy;
                btnCompareResults.Enabled = !isBusy;
                btnLoadResults.Enabled = !isBusy;
                btnBuildGraph.Enabled = !isBusy;
                btnCancel.Visible = isBusy;
                progressBar.Visible = isBusy;
                progressBar.Value = 0;
                lblStatus.Text = isBusy ? "Выполнение операции..." : "Готово";
            });
        }
    }
}