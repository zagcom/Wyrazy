﻿using System;
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
using static System.IO.Directory;
using static System.IO.Path;
using static System.Environment;

namespace Wyrazy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CheckDirectory();
            ListAllFiles();
        }

        public void CheckDirectory()
        {

            string workingDirectory = AppDir();
            Info.Content = workingDirectory;
            if (!Exists(workingDirectory)) {
                Info.Content = Info.Content + "\n" + "Directory not existing => Creating new directory";
                CreateDirectory(workingDirectory);
            } else
            {
                Info.Content = Info.Content + "\n" + "Directory exist";
            }

        }
        private void CreateNewList_Click(object sender, RoutedEventArgs e)
        {
            var fileName = Combine(AppDir(), NewListName.Text + ".txt").ToUpper();
            Info.Content = Info.Content + "\n" + fileName;
            if (!File.Exists(fileName))
            {
                Info.Content = Info.Content + "\n" + "File not existing => Creating new file";
                StreamWriter sw = File.CreateText(fileName);
                sw.Close();
                FilesList.Items.Add(NewListName.Text.ToUpper());
            }
            else
            {
                MessageBox.Show("Lista już istnieje", "Uwaga", MessageBoxButton.OK,MessageBoxImage.Warning);
                Info.Content = Info.Content + "\n" + "File exist";
            }
            //ListAllFiles();

        }

        private string AppDir()
        {
            string personalDirectory = GetFolderPath(SpecialFolder.Personal);
            var appDirectory = new string[] { personalDirectory, "Wyrazy", "Listy" };
            string workingDirectory = Combine(appDirectory);
            return workingDirectory;
        }

        private void ListAllFiles()
        {
            var direct = AppDir();
            var ListOfFiles = Directory.GetFiles(direct);
           
            foreach (string file in ListOfFiles){
            var fileName = GetFileNameWithoutExtension(file).ToUpper();
                if (!FilesList.Items.Contains(fileName)) 
                    FilesList.Items.Add(fileName);       
            }
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (FilesList.SelectedItem!=null) { 
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wybraną listę?", "Uwaga", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes){
                var direct = AppDir();
                var filename = FilesList.SelectedItem.ToString();
                var filepath = Combine(direct,filename + ".TXT");
                if (File.Exists(filepath)){
                    File.Delete(filepath);
                    FilesList.Items.Remove(filename);
                }
                //ListAllFiles();
            }
            }
            else
            {
                MessageBox.Show("Aby usunąć listę musisz ją zaznaczyć powyżej.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void FilesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordsList.Items.Clear();
            var direct = AppDir();
            if (FilesList.SelectedItem != null)
            {
                var filename = FilesList.SelectedItem.ToString();
                var filepath = Combine(direct, filename + ".TXT");
                string line;
                int i = 0;
                if (File.Exists(filepath))
                {
                    StreamReader words = File.OpenText(filepath);
                    while ((line = words.ReadLine()) != null)
                    {
                        WordsList.Items.Add(line);
                        i++;
                    }
                    words.Close();
                }
            }
        }

        private void CreateNewWord_Click(object sender, RoutedEventArgs e)
        {
            var direct = AppDir();
            var filename = FilesList.SelectedItem.ToString();
            var filepath = Combine(direct, filename + ".TXT");
            string line = NewWord.Text.ToUpper();
            string exlines;
            int flag = 0;
            if (File.Exists(filepath))
            {
                if (RepetedWords.IsChecked == false)
                {
                    StreamReader words = File.OpenText(filepath);
                    while ((exlines = words.ReadLine()) != null)
                    {
                        if (exlines == line) 
                        {
                            
                            flag++;
                        }
                    }
                    words.Close();
                    if (flag == 0)
                    {
                        AddWord(filepath, line);
                    }
                    else
                    {
                        MessageBox.Show("Podany wyraz już istnieje na liście.", "Uwaga", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    
                } 
                else
                {
                    AddWord(filepath, line);
                    
                }
            }
        }

        private void AddWord(string filepath, string line)
        {
            StreamWriter sw = new StreamWriter(filepath, true);
            sw.WriteLine(line);
            sw.Close();
            WordsList.Items.Add(line);
            NewWord.Text = "Dodaj nowy wyraz";
        }

        private void WordsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WordsList.SelectionMode = SelectionMode.Extended;
        }

        private void MoveObject_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in WordsList.SelectedItems)
            {
                SelectedWords.Items.Add(item);
            }
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in WordsList.Items)
            {
                WordsList.SelectionMode = SelectionMode.Extended;
                WordsList.SelectedItems.Add(item);
            }
        }
    }
}