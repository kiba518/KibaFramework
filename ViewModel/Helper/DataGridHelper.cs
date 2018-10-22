using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Utility; 

namespace ViewModel
{
    public static class DataGridHelper
    {
        public static T FindVisualChildByName<T>(Visual parent, string name) where T : Visual
        {
            if (parent != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i) as Visual;
                    string controlName = child.GetValue(Control.NameProperty) as string;
                    if (controlName == name)
                    {
                        return child as T;
                    }
                    else
                    {
                        T result = FindVisualChildByName<T>(child, name);
                        if (result != null)
                            return result;
                    }
                }
            }
            return null;
        }
        /// <summary>  
        /// 获取DataGrid控件单元格  
        /// </summary>  
        /// <param name="dataGrid">DataGrid控件</param>  
        /// <param name="rowIndex">单元格所在的行号</param>  
        /// <param name="columnIndex">单元格所在的列号</param>  
        /// <returns>指定的单元格</returns>  
        public static DataGridCell GetCell(this DataGrid dataGrid, int rowIndex, int columnIndex)
        {
            DataGridRow rowContainer = dataGrid.GetRow(rowIndex);
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter != null)
                {
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    if (cell == null)
                    {
                        //dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[columnIndex]);
                        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    }               
                    return cell;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取DataGrid控件单元格  
        /// </summary>
        /// <param name="rowContainer">DataGrid选中行</param>
        /// <param name="columnIndex">单元格所在的列号</param>
        /// <returns>指定的单元格</returns>
        public static DataGridCell GetCellByRow(DataGridRow rowContainer, int columnIndex)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter != null)
                {
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    return cell;
                }
            }
            return null;
        }
        /// <summary>  
        /// 获取DataGrid的行  
        /// </summary>  
        /// <param name="dataGrid">DataGrid控件</param>  
        /// <param name="rowIndex">DataGrid行号</param>  
        /// <returns>指定的行号</returns>  
        public static DataGridRow GetRow(this DataGrid dataGrid, int rowIndex)
        {
            DataGridRow rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            if (rowContainer == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[rowIndex]);
                rowContainer = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex);
            }
            return rowContainer;
        }

        /// <summary>  
        /// 获取父可视对象中第一个指定类型的子可视对象  
        /// </summary>  
        /// <typeparam name="T">可视对象类型</typeparam>  
        /// <param name="parent">父可视对象</param>  
        /// <returns>第一个指定类型的子可视对象</returns>  
        public static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
        public static string GetCellTextBlock(this DataGrid dataGrid, string name, int Col)
        {
            string oldValue = "";
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBlock textOld = objOld as TextBlock;
                oldValue = textOld.Text;
            }
            return oldValue;
        }
        public static void SetCellTextBlock(this DataGrid dataGrid, string name, int Col, string value)
        {
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBlock textOld = objOld as TextBlock;
                textOld.Text = value;
            }
        }
        public static string GetCellTextBox(this DataGrid dataGrid, string name, int Col)
        {
            string oldValue = "";
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellEditingTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBox textOld = objOld as TextBox;
                oldValue = textOld.Text;
            }
            return oldValue;
        }
        public static void SetCellTextBox(this DataGrid dataGrid, string name, int Col, string value)
        {
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellEditingTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBox textOld = objOld as TextBox;
                textOld.Text = value;
            }
        }
        public static void SetCellTextBoxFocus(this DataGrid dataGrid, string name, int Col)
        {
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellEditingTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBox textOld = objOld as TextBox;
                textOld.Focus();
            }
        }
        public static void SetCellTextBoxAndFocus(this DataGrid dataGrid, string name, int Col, string value)
        {
            object itemOld = dataGrid.CurrentCell.Item;
            DataGridTemplateColumn templeColumnOld = dataGrid.Columns[Col] as DataGridTemplateColumn;
            FrameworkElement elementOld = templeColumnOld.GetCellContent(itemOld);
            object objOld = templeColumnOld.CellEditingTemplate.FindName(name, elementOld);
            if (objOld != null)
            {
                TextBox textOld = objOld as TextBox;
                textOld.Text = value;   
                int i = 0;
                while (!textOld.Focus() || i > 100)
                {
                    i++;
                }
            }
        }

        public static void MoveFocus(this DataGrid DataGrid1, string newName, string newValueName, int rowPush, int colPush)
        {
            Int32 row = DataGrid1.Items.IndexOf(DataGrid1.CurrentItem);
            Int32 Col = DataGrid1.Columns.IndexOf(DataGrid1.CurrentColumn);
            if ((row + rowPush) < DataGrid1.Items.Count)
            {
                DataGrid1.CancelEdit(DataGridEditingUnit.Cell);
                DataGrid1.SelectedItem = DataGrid1.Items[row + rowPush];
                var cell = GetCell(DataGrid1, row + rowPush, Col + colPush);
                if (cell != null)
                {
                    int i = 0;
                    while (!cell.Focus() || i > 100)
                    {
                        i++;
                    }
                    string newValue = GetCellTextBlock(DataGrid1, newValueName, Col + colPush);
                    DataGrid1.BeginEdit();
                    SetCellTextBoxAndFocus(DataGrid1, newName, Col + colPush, newValue);
                }
            }
        }
       
      
    }
}
