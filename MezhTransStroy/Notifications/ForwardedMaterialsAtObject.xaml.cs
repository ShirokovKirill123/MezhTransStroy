using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using DocumentFormat.OpenXml;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using MezhTransStroy.Database;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для ForwardedMaterialsAtObject.xaml
    /// </summary>
    public partial class ForwardedMaterialsAtObject : Page
    {
        private List<string> forwardedMaterialsList = new List<string>();

        public ForwardedMaterialsAtObject()
        {
            InitializeComponent();
            ForwardedMaterials();
        }

        private void ForwardedMaterials()
        {
            using (var context = new СтроительствоEntities())
            {
                forwardedMaterialsList = context.История_Перемещений_Материалов
                    .Where(h => h.Описание.Contains("отправлен со склада"))
                    .OrderBy(h => h.id)
                    .Select(h => h.Описание)
                    .ToList();

                // Исключение удалённых ранее записей
                var displayList = forwardedMaterialsList
                    .Where(item => !MaterialManager.excludedMaterialsList.Contains(item))
                    .ToList();

                ForwardedMaterialsList.ItemsSource = displayList.Any()
                    ? displayList
                    : new List<string> { "Нет перенаправленных материалов" };
            }
        }        

        private void ClearAllForwardedMaterials_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
            "Вы уверены, что хотите удалить все записи об отправке материалов на объект?",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Добавление всех записей в исключённые
                MaterialManager.excludedMaterialsList.AddRange(forwardedMaterialsList);
                MaterialManager.SaveExcludedMaterials();  

                ForwardedMaterialsList.ItemsSource = new List<string> { "Нет перенаправленных материалов" };

                MessageBox.Show("Все записи удалены из списка", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearSelectedNotification_Click(object sender, RoutedEventArgs e)
        {
            var selectedRecord = ForwardedMaterialsList.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedRecord) || selectedRecord == "Нет перенаправленных материалов")
            {
                MessageBox.Show("Выберите запись для удаления", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить выбранную запись?\n\n{selectedRecord}",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Добавление выбранной записи в исключённые
                MaterialManager.excludedMaterialsList.Add(selectedRecord);
                MaterialManager.SaveExcludedMaterials();  

                var displayList = forwardedMaterialsList
                    .Where(item => !MaterialManager.excludedMaterialsList.Contains(item))
                    .ToList();

                ForwardedMaterialsList.ItemsSource = null;
                ForwardedMaterialsList.ItemsSource = displayList.Any()
                    ? displayList
                    : new List<string> { "Нет перенаправленных материалов" };

                MessageBox.Show("Запись из списка удалена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (ForwardedMaterialsList.Items.Count == 0 ||
        (ForwardedMaterialsList.Items.Count == 1 && ForwardedMaterialsList.Items[0].ToString() == "Нет перенаправленных материалов"))
            {
                MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word документы (*.docx)|*.docx",
                FileName = "Перенаправленные_Материалы.docx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();                 

                    // Заголовок отчёта
                    var titleParagraph = new Paragraph();
                    var titleRun = new Run();
                    var titleRunProperties = new RunProperties();

                    titleRunProperties.Bold = new Bold();
                    titleRunProperties.FontSize = new FontSize() { Val = "28" };
                    titleRunProperties.RunFonts = new RunFonts() { Ascii = "Times New Roman" };

                    titleRun.Append(titleRunProperties);
                    titleRun.Append(new Text("Отчёт по перенаправленным материалам"));

                    titleParagraph.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "400" }));
                    titleParagraph.Append(titleRun);

                    body.Append(titleParagraph);
                    body.Append(new Paragraph(new Run(new Text(""))));

                    // Список записей
                    int number = 1;
                    foreach (var item in ForwardedMaterialsList.Items)
                    {
                        var para = new Paragraph(new Run(new Text($"{number}. {item}")))
                        {
                            ParagraphProperties = new ParagraphProperties(
                                new SpacingBetweenLines() { After = "100" }
                            )
                        };
                        body.Append(para);
                        number++;
                    }

                    body.Append(new Paragraph(new Run(new Text(""))));

                    // Пользователь, дата и подпись
                    using (var context = new СтроительствоEntities())
                    {
                        int employeeId = Manager.User.Employee;
                        var сотрудник = context.Сотрудники
                            .Where(emp => emp.id == employeeId)
                            .Select(emp => new { emp.ФИО, emp.id_Отдела })
                            .FirstOrDefault();

                        string fio = сотрудник?.ФИО ?? "Неизвестный пользователь";
                        string отдел = сотрудник != null
                            ? context.Отделы.Where(o => o.id == сотрудник.id_Отдела).Select(o => o.Название).FirstOrDefault() ?? "Неизвестный отдел"
                            : "Неизвестный отдел";

                        var confirmationParagraph = new Paragraph(new Run(new Text(
                            $"Я, {fio} ({отдел}), подтверждаю перенаправление материала(ов) на объект(ы).")))
                        {
                            ParagraphProperties = new ParagraphProperties(
                                new SpacingBetweenLines() { After = "200" }
                            )
                        };

                        body.Append(confirmationParagraph);
                    }

                    var dateParagraph = new Paragraph(new Run(new Text($"Дата: {DateTime.Now:dd.MM.yyyy}")))
                    {
                        ParagraphProperties = new ParagraphProperties(
                                new SpacingBetweenLines() { After = "200" }
                            )
                    };
                    body.Append(dateParagraph);

                    var signatureParagraph = new Paragraph(new Run(new Text("Подпись:")));
                    body.Append(signatureParagraph);

                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                MessageBox.Show("Отчёт успешно экспортирован в документ", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
