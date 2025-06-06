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
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Win32;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using MezhTransStroy.Database;

namespace MezhTransStroy
{
    /// <summary>
    /// Логика взаимодействия для MaterialMovementsPage.xaml
    /// </summary>
    public partial class MaterialMovementsPage : Page
    {
        public MaterialMovementsPage()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            using (var context = new СтроительствоEntities())
            {
                var историяМатериалов = context.История_Перемещений_Материалов
                    .OrderBy(h => h.id)
                    .Select(h => "[Материал] " + h.Описание)
                    .ToList();

                var историяОборудования = context.История_Перемещений_Оборудования
                    .OrderBy(h => h.id)
                    .Select(h => "[Оборудование] " + h.Описание)
                    .ToList();

                var общаяИстория = историяМатериалов
                    .Concat(историяОборудования)
                    .OrderBy(s => s) 
                    .ToList();

                NotificationList.ItemsSource = общаяИстория.Any()
                    ? общаяИстория
                    : new List<string> { "История пуста" };
            }
        }

        private void ClearNotifications_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите очистить всю историю движений?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new СтроительствоEntities())
                {
                    context.История_Перемещений_Материалов.RemoveRange(context.История_Перемещений_Материалов);
                    context.История_Перемещений_Оборудования.RemoveRange(context.История_Перемещений_Оборудования);
                    context.SaveChanges();
                }

                LoadHistory();
                MessageBox.Show("История движений успешно очищена", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ExportToWord_Click(object sender, RoutedEventArgs e)
        {
            if (NotificationList.Items.Count == 0 ||
                (NotificationList.Items.Count == 1 && NotificationList.Items[0].ToString() == "История пуста"))
            {
                MessageBox.Show("Нет данных для экспорта", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Word документы (*.docx)|*.docx",
                FileName = "История_Движений.docx"
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
                    titleRun.Append(new Text("Отчёт по истории движений ресурсов"));

                    titleParagraph.Append(new ParagraphProperties(new Justification() { Val = JustificationValues.Center }, new SpacingBetweenLines() { After = "400" }));
                    titleParagraph.Append(titleRun);

                    body.Append(titleParagraph);
                    body.Append(new Paragraph(new Run(new Text(""))));

                    // Список движений
                    int number = 1;
                    foreach (var item in NotificationList.Items)
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
                        var fio = context.Сотрудники
                            .Where(emp => emp.id == employeeId)
                            .Select(emp => emp.ФИО)
                            .FirstOrDefault() ?? "Неизвестный пользователь";

                        var userParagraph = new Paragraph(new Run(new Text($"Пользователь: {fio}")))
                        {
                            ParagraphProperties = new ParagraphProperties(
                                new SpacingBetweenLines() { After = "200" }
                            )
                        };
                        body.Append(userParagraph);
                    }

                    var dateParagraph = new Paragraph(new Run(new Text($"Дата: {DateTime.Now:dd.MM.yyyy}")))
                    {
                        ParagraphProperties = new ParagraphProperties(
                                new SpacingBetweenLines() { After = "200" }
                            )
                    };
                    body.Append(dateParagraph);

                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                MessageBox.Show("История успешно экспортирована в документ", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
