using AutoLotModel2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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

namespace Iscu_Paula_Lab6
{
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing,
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customer1.Local;
            ctx.Customer1.Load();
            System.Windows.Data.CollectionViewSource order1ViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("order1ViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // order1ViewSource.Source = [generic data source]
            ctx.Order1.Load();
            ctx.Inventory1.Load();
            cmbCustomer1.ItemsSource = ctx.Customer1.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomer1.DisplayMemberPath = "FirstName";
            cmbCustomer1.SelectedValuePath = "CustId";
            cmbInventory1.ItemsSource = ctx.Inventory1.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbInventory1.DisplayMemberPath = "Make";
            cmbInventory1.SelectedValuePath = "CarId";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer1 customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer1()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customer1.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
                if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer1)customer1DataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
            }
            else
                if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer1)customer1DataGrid.SelectedItem;
                    ctx.Customer1.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
            }
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void order1DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbCustomer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void btnSaveO_Click(object sender, RoutedEventArgs e)
        {
            Order1 order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer1 customer = (Customer1)cmbCustomer1.SelectedItem;
                    Inventory1 inventory = (Inventory1)cmbInventory1.SelectedItem;
                    //instantiem Order entity
                    order = new Order1()
                    {

                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Order1.Add(order);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void BindDataGrid()
            {
                var queryOrder = from ord in ctx.Order1
                                 join cust in ctx.Customer1 on ord.CustId equals
                                 cust.CustId
                                 join inv in ctx.Inventory1 on ord.CarId
                     equals inv.CarId
                                 select new
                                 {
                                     ord.OrderId,
                                     ord.CarId,
                                     ord.CustId,
                                     cust.FirstName,
                                     cust.LastName,
                                     inv.Make,
                                     inv.Color
                                 };
                customerViewSource.Source = queryOrder.ToList();
            }
        }
    }

           
