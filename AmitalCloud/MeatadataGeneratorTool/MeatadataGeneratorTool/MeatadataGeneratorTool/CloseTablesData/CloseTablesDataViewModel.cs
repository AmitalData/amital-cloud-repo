using GalaSoft.MvvmLight.Command;
using MeatadataGeneratorTool.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MeatadataGeneratorTool.CloseTablesData
{
    public class CloseTablesDataViewModel : PropertyChangedImplementation
    {
        ObjectTableViewModel ViewModel;
        public FieldsValues FieldsValues;

        //public ObservableCollection<Row> rows { get; set; } 
        public CloseTablesDataViewModel(ObjectTableViewModel OTViewModel)
        {
            ViewModel = OTViewModel;
        }

        public bool IsEditMode { get; set; }
        public CloseTablesDataViewModel(ObjectTableViewModel OTViewModel, bool isEditMode)
        {
            this.IsEditMode = isEditMode;
            ViewModel = OTViewModel;
        }

        Grid grid = null;
        public Grid CLoseTableFields
        {
            get
            {
                if (grid == null)
                {
                    grid = new Grid() { MinHeight = 50, MinWidth = 50 };
                    //grid.Background = new LinearGradientBrush(Colors.Yellow, Colors.Yellow, 1);
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                    int row = 0;
                    foreach (var objectField in ViewModel.ObsList)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32), });

                        TextBlock textBlock = new TextBlock();
                        textBlock.Text = objectField.FieldName;
                        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                        textBlock.VerticalAlignment = VerticalAlignment.Center;
                        textBlock.Margin = new Thickness() { Right = 10, Top = 5, Left = 10, Bottom = 5 };
                        Grid.SetColumn(textBlock, 0);
                        Grid.SetRow(textBlock, row);
                        grid.Children.Add(textBlock);

                        #region Text, Double, Decimal, Integer
                        if (objectField.FieldDataType.Trim() == "Text" || objectField.FieldDataType.Trim() == "nText" || objectField.FieldDataType.Trim() == "Double" || objectField.FieldDataType.Trim() == "Decimal" || objectField.FieldDataType.Trim() == "Integer" || objectField.FieldDataType.Trim() == "SigDouble" || objectField.FieldDataType.Trim() == "UnsDecimal" || objectField.FieldDataType.Trim() == "UnsInteger")
                        {
                            TextBox txtControl = new TextBox();
                            txtControl.Tag = objectField;
                            txtControl.Name = objectField.FieldName;
                            txtControl.Width = 250;
                            txtControl.Margin = new Thickness() { Right = 10, Top = 5, Left = 10, Bottom = 5 };
                            Grid.SetColumn(txtControl, 2);
                            Grid.SetRow(txtControl, row);

                            if (IsEditMode && ViewModel.FieldsDictionary.ContainsKey(objectField.FieldName))
                            {
                                txtControl.Text = ViewModel.FieldsDictionary[objectField.FieldName];
                            }

                            txtControl.TextChanged += (ss1, ee1) =>
                            {
                                TextBox t = ss1 as TextBox;
                                string value = t.Text;

                                if (ViewModel.FieldsDictionary.Keys.Contains(objectField.FieldName))
                                {
                                    ViewModel.FieldsDictionary[objectField.FieldName] = value;
                                }
                                else
                                {
                                    ViewModel.FieldsDictionary.Add(objectField.FieldName, value);
                                }


                            };

                            
                            grid.Children.Add(txtControl);
                        }
                        #endregion

                        #region Boolean
                        if (objectField.FieldDataType.Trim() == "Boolean")
                        {
                            CheckBox chkControl = new CheckBox();
                            chkControl.Tag = objectField.FieldName;
                            chkControl.Margin = new Thickness() { Right = 10, Top = 5, Left = 10, Bottom = 5 };
                            Grid.SetColumn(chkControl, 2);
                            Grid.SetRow(chkControl, row);
                            //FieldsValues.SetFieldValue(objectField.Id, null);
                            

                            if (IsEditMode && ViewModel.FieldsDictionary.ContainsKey(objectField.FieldName))
                            {
                                chkControl.IsChecked = (ViewModel.FieldsDictionary[objectField.FieldName]) == "true" ;
                            }
                            else
                            {
                                if (ViewModel.FieldsDictionary.Keys.Contains(objectField.FieldName))
                                {
                                    ViewModel.FieldsDictionary[objectField.FieldName] = "false";
                                }
                            }


                            chkControl.Checked += (sender, e) =>
                            {
                                CheckBox chk = sender as CheckBox;
                                if (ViewModel.FieldsDictionary.Keys.Contains(objectField.FieldName))
                                {
                                    ViewModel.FieldsDictionary[objectField.FieldName] = "true";
                                }
                                else
                                {
                                    ViewModel.FieldsDictionary.Add(objectField.FieldName, "true");
                                }

                            };

                            chkControl.Unchecked += (sender, e) =>
                            {
                                CheckBox chk = sender as CheckBox;
                                if (ViewModel.FieldsDictionary.Keys.Contains(objectField.FieldName))
                                {
                                    ViewModel.FieldsDictionary[objectField.FieldName] = "false";
                                }
                                else
                                {
                                    ViewModel.FieldsDictionary.Add(objectField.FieldName, "false");
                                }
                            };


                            

                            grid.Children.Add(chkControl);
                        }
                        #endregion
                        row++;
                    }
                }
                return grid;
            }
        }

        public RelayCommand OkBtnCommand
        {
            get { return new RelayCommand(() => this.OkBtnMethod()); }
        }

        public RelayCommand CancelBtnCommand
        {
            get { return new RelayCommand(() => this.CancelBtnMethod()); }
        }

        private void CancelBtnMethod()
        {
            ViewModel.TableDataWindow.Close();
        }

        private void OkBtnMethod()
        {

            ErrorMessages = string.Empty;

            if (!IsEditMode)
            {
                if (ViewModel.rows == null)
                {
                    ViewModel.rows = new ObservableCollection<Row>();
                }

                Row newDataRow = new Row();


                bool codeExists = ViewModel.rows.Any(row => row._data.TryGetValue("Code", out object codeValue) && codeValue?.ToString() == ViewModel.FieldsDictionary["Code"].ToString());
                if (codeExists)
                {
                    ErrorMessages = "Code Already Exist";
                }

                else if (ViewModel.FieldsDictionary.Count < ViewModel.CLoseTableDataGrid.Columns.Count)
                {
                    ErrorMessages = "All Fields Are Required";
                }
                else if (ViewModel.FieldsDictionary.Count > ViewModel.CLoseTableDataGrid.Columns.Count)
                {
                    //var itemWithMaxColumns = ViewModel.rows.OrderByDescending(r => r._data.Count).FirstOrDefault();
                    //if (newDataRow._data.Count > itemWithMaxColumns._data.Count)
                    //{
                    //    var missingDataRows = ViewModel.rows.Where(r => r._data.Count < newDataRow._data.Count);
                    //    foreach (var item in missingDataRows)
                    //    {
                    //        var list = newDataRow._data.Where(d => !item._data.ContainsKey(d.Key)).ToList();
                    //        foreach (var missedData in list)
                    //        {
                    //            item._data.Add(missedData.Key, null);
                    //        }

                    //    }
                    //}

                    AddMissingDataColumns();
                }

                if (ErrorMessages == "")
                {
                    foreach (var item in ViewModel.FieldsDictionary)
                    {
                        newDataRow[item.Key] = item.Value;
                    }
                    ViewModel.rows.Add(newDataRow);

                    if (ViewModel.CLoseTableDataGrid.Columns.Count == 0)
                    {
                        foreach (var item in ViewModel.FieldsDictionary)
                        {
                            var TempColumn = new DataGridTextColumn() { MinWidth = 120 };
                            TempColumn.Header = item.Key;
                            Binding bind = new Binding();
                            bind.Mode = BindingMode.OneWay;
                            bind.Converter = new RowIndexConverter();
                            bind.ConverterParameter = item.Key;

                            TempColumn.Binding = bind;
                            ViewModel.CLoseTableDataGrid.Columns.Add(TempColumn);
                        }
                    }


                    ViewModel.CLoseTableDataGrid.ItemsSource = ViewModel.rows;
                    ViewModel.TableDataWindow.Close();
                }
                else
                {
                    ErrorsVisibility = Visibility.Visible;
                }

            }
            else
            {

                if (ViewModel.FieldsDictionary.Count < ViewModel.CLoseTableDataGrid.Columns.Count)
                {
                    ErrorMessages = "All Fields Are Required";
                }

                if (ErrorMessages == "")
                {
                    var rowData = (ViewModel.CLoseTableDataGrid.SelectedItem as Row)._data;
                    foreach (var k in ViewModel.FieldsDictionary.Keys)
                    {
                        if (rowData.ContainsKey(k))
                            rowData[k] = ViewModel.FieldsDictionary[k];
                        else
                            rowData.Add(k, ViewModel.FieldsDictionary[k]);
                    }

                    AddMissingDataColumns();
                    ViewModel.CLoseTableDataGrid.ItemsSource = ViewModel.rows;
                    ViewModel.BuildRowsData(ViewModel.rows.ToList());
                    ViewModel.TableDataWindow.Close();
                }
                else
                {
                    ErrorsVisibility = Visibility.Visible;
                }
            }

        }

        private void AddMissingDataColumns()
        {
            var newColumns = ViewModel.FieldsDictionary.Where(f => !ViewModel.CLoseTableDataGrid.Columns.Any(c => c.Header.ToString() == f.Key));
            foreach (var newColumnItem in newColumns)
            {
                var rowsWithoutNewColumn = ViewModel.rows.Where(r => !r._data.ContainsKey(newColumnItem.Key));
                foreach (var rowItem in rowsWithoutNewColumn)
                {
                    rowItem._data.Add(newColumnItem.Key, null);
                }
                var TempColumn = new DataGridTextColumn() { MinWidth = 120 };
                TempColumn.Header = newColumnItem.Key;
                Binding bind = new Binding();
                bind.Mode = BindingMode.OneWay;
                bind.Converter = new RowIndexConverter();
                bind.ConverterParameter = newColumnItem.Key;
                TempColumn.Binding = bind;
                ViewModel.CLoseTableDataGrid.Columns.Add(TempColumn);


            }
        }

        Visibility errorsVisibility = Visibility.Collapsed;
        public Visibility ErrorsVisibility
        {
            get { return errorsVisibility; }
            set { errorsVisibility = value; FirePropertyChanged("ErrorsVisibility"); }
        }

        public string errorMessages;
        public string ErrorMessages
        {
            get
            {
                return errorMessages;
            }
            set
            {
                errorMessages = value;
                FirePropertyChanged("ErrorMessages");
            }
        }
    }

    public class Row : PropertyChangedImplementation
    {
        public Dictionary<string, object> _data = new Dictionary<string, object>();
        public object this[string index]
        {
            get
            {
                return _data.ContainsKey(index) ? _data[index] : null;

            }
            set { _data[index] = value; }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        public RelayCommand DeleteBtnCommand
        {
            get { return new RelayCommand(() => this.DeleteBtnMethod()); }
        }

        private void DeleteBtnMethod()
        {
            //ViewModel.TableDataWindow.Close();
        }
    }
}
