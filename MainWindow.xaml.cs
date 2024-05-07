using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SimpleNoteApp
{
    public partial class MainWindow : Window
    {
        // Standardverzeichnis für die Notizen
        private readonly string notizenVerzeichnis = "C:\\KMS_1_06_LE_06_02_Notizen";

        public MainWindow()
        {
            InitializeComponent();
            NotizenInListeLaden(); // Beim Start das Verzeichnis überprüfen und Dateien in der ListBox anzeigen
        }

        /// <summary>
        /// Lädt die Notizen aus dem Verzeichnis und fügt sie in die ListBox ein.
        /// </summary>
        private void NotizenInListeLaden()
        {
            txtblk_meldungen.Text = string.Empty;
            txtblk_meldungen.Foreground = Brushes.Red;

            try
            {
                if (!Directory.Exists(notizenVerzeichnis))
                {
                    Directory.CreateDirectory(notizenVerzeichnis);
                }

                lstbx_vorhandene_notizen.Items.Clear(); // ListBox leeren

                var notizen = Directory.GetFiles(notizenVerzeichnis, "*.txt");

                foreach (var notiz in notizen)
                {
                    // Nur den Dateinamen ohne Erweiterung in die ListBox einfügen
                    var dateiName = Path.GetFileNameWithoutExtension(notiz);
                    lstbx_vorhandene_notizen.Items.Add(dateiName);
                }
            }
            catch (UnauthorizedAccessException)
            {
                txtblk_meldungen.Text = "Zugriff auf das Verzeichnis aufgrund fehlender Berechtigung nicht möglich.";
            }
            catch (IOException ex)
            {
                txtblk_meldungen.Text = $"Ein E/A-Fehler ist aufgetreten: {ex.Message}.";
            }
            catch (Exception ex)
            {
                txtblk_meldungen.Text = $"Ein unbekannter Fehler ist aufgetreten: {ex.Message}.";
            }
        }

        /// <summary>
        /// Methode für den Klick auf den Button "Notiz erstellen"
        /// </summary>
        private void btn_notiz_erstellen_Click(object sender, RoutedEventArgs e)
        {
            txtblk_meldungen.Text = string.Empty;
            txtblk_meldungen.Foreground = Brushes.Red;

            bool istTitelLeer = string.IsNullOrWhiteSpace(txt_notiz_titel.Text);
            bool istInhaltLeer = string.IsNullOrWhiteSpace(txt_notiz_inhalt.Text);

            if (istTitelLeer && istInhaltLeer)
            {
                txtblk_meldungen.Text = "Sie müssen einen Titel und einen Inhalt eingeben!";
            }
            else if (istTitelLeer)
            {
                txtblk_meldungen.Text = "Bitte geben Sie einen Titel ein!";
            }
            else if (istInhaltLeer)
            {
                txtblk_meldungen.Text = "Der Inhalt der Notiz darf nicht leer sein!";
            }
            else
            {
                try
                {
                    // Inhalt der beiden Textfelder auslesen
                    string titel = txt_notiz_titel.Text;
                    string inhalt = txt_notiz_inhalt.Text;

                    // Vollständiger Pfad für die Erstellung der neuen Datei
                    string dateiPfad = Path.Combine(notizenVerzeichnis, $"{titel}.txt");

                    // Neue Datei schreiben
                    File.WriteAllText(dateiPfad, inhalt);

                    // Erfolgsmeldung ausgeben
                    txtblk_meldungen.Foreground = Brushes.Green;
                    txtblk_meldungen.Text = "Notiz erfolgreich erstellt!";

                    // Liste der vorhandenen Notizen aktualisieren
                    NotizenInListeLaden();

                    // Inhalte der Textfelder leeren
                    txt_notiz_titel.Text = string.Empty;
                    txt_notiz_inhalt.Text = string.Empty;
                }
                catch (UnauthorizedAccessException)
                {
                    txtblk_meldungen.Text = "Sie haben keine Berechtigung, diese Datei zu erstellen.";
                }
                catch (DirectoryNotFoundException)
                {
                    txtblk_meldungen.Text = "Das angegebene Verzeichnis wurde nicht gefunden.";
                }
                catch (PathTooLongException)
                {
                    txtblk_meldungen.Text = "Der Dateipfad ist zu lang.";
                }
                catch (IOException ex)
                {
                    txtblk_meldungen.Text = $"Ein E/A-Fehler ist aufgetreten: {ex.Message}.";
                }
                catch (Exception ex)
                {
                    txtblk_meldungen.Text = $"Ein unbekannter Fehler ist aufgetreten: {ex.Message}.";
                }
            }
        }

        private void lstbx_vorhandene_notizen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstbx_vorhandene_notizen.SelectedItem != null)
            {
                try
                {
                    string ausgewaehlteNotiz = lstbx_vorhandene_notizen.SelectedItem.ToString();
                    string dateiPfad = Path.Combine(notizenVerzeichnis, $"{ausgewaehlteNotiz}.txt");

                    string inhalt = File.ReadAllText(dateiPfad);

                    txt_notiz_titel.Text = ausgewaehlteNotiz;
                    txt_notiz_inhalt.Text = inhalt;

                    txtblk_meldungen.Foreground = Brushes.Green;
                    txtblk_meldungen.Text = "Notiz erfolgreich geladen!";
                }
                catch (FileNotFoundException)
                {
                    txtblk_meldungen.Text = "Die ausgewählte Notiz wurde nicht gefunden.";
                }
                catch (IOException ex)
                {
                    txtblk_meldungen.Text = $"Ein E/A-Fehler ist aufgetreten: {ex.Message}.";
                }
                catch (Exception ex)
                {
                    txtblk_meldungen.Text = $"Ein unbekannter Fehler ist aufgetreten: {ex.Message}.";
                }
            }
        }

        private void btn_notiz_loeschen_Click(object sender, RoutedEventArgs e)
        {
            if (lstbx_vorhandene_notizen.SelectedItem == null)
            {
                txtblk_meldungen.Foreground = Brushes.Red;
                txtblk_meldungen.Text = "Sie müssen eine Notiz auswählen!";
                return;
            }

            try
            {
                string ausgewaehlteNotiz = lstbx_vorhandene_notizen.SelectedItem.ToString();
                string dateiPfad = Path.Combine(notizenVerzeichnis, $"{ausgewaehlteNotiz}.txt");

                File.Delete(dateiPfad);

                // Liste der vorhandenen Notizen aktualisieren
                NotizenInListeLaden();

                txtblk_meldungen.Foreground = Brushes.Blue;
                txtblk_meldungen.Text = "Notiz erfolgreich gelöscht!";

                // Inhalte der Textfelder leeren
                txt_notiz_titel.Text = string.Empty;
                txt_notiz_inhalt.Text = string.Empty;
            }
            catch (FileNotFoundException)
            {
                txtblk_meldungen.Text = "Die ausgewählte Notiz konnte nicht gefunden werden.";
            }
            catch (UnauthorizedAccessException)
            {
                txtblk_meldungen.Text = "Keine Berechtigung zum Löschen der Notiz.";
            }
            catch (IOException ex)
            {
                txtblk_meldungen.Text = $"Ein E/A-Fehler ist aufgetreten: {ex.Message}.";
            }
            catch (Exception ex)
            {
                txtblk_meldungen.Text = $"Ein unbekannter Fehler ist aufgetreten: {ex.Message}.";
            }
        }
    }
}
