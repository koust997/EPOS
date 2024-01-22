using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EPOS_System
{
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
            this.Text = "Reports";
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            // Calling a method to display inventory
            Inventory_Display();
            // Calling a method to handle opening stock
            OpeningStock();
            // Calling a method to generate a random number
            RandomGen(8);

            //Displaying a message 
            MessageBox.Show("Report Generated", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Inventory_Display()
        {
            // Clear the ListBox before adding new items
            Inventory_ListBox.Items.Clear();

            string inventory = "Total_Inventory.txt";
            // Try to open the inventory file for reading
            try
            {
                using (StreamReader inputFile = File.OpenText(inventory))
                // Open the file using a StreamReader object
                {
                    // Read each line from the file
                    while (!inputFile.EndOfStream)
                    {
                        string input = inputFile.ReadLine();
                        // Split each line by comma to separate the item details
                        string[] line = input.Split(',');

                        // Check if the array has enough elements before accessing them
                        if (line.Length >= 5)
                        {
                            // Add the line to the ListBox
                            Inventory_ListBox.Items.Add($"{line[0]}, {line[1]}, {line[2]}, {line[3]}, {line[4]}, {line[5]}, {line[6]}");
                        }
                        else
                        {
                            // Handle the case where line[] doesn't have enough elements
                            MessageBox.Show("Invalid data format in the file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                // Calculate and display total available size stock
                int totalSize1 = CalculateTotal(1);
                Size_4.Text = totalSize1.ToString();

                int totalSize2 = CalculateTotal(2);
                Size_5.Text = totalSize2.ToString();

                int totalSize3 = CalculateTotal(3);
                Size_6.Text = totalSize3.ToString();

                int totalSize4 = CalculateTotal(4);
                Size_7.Text = totalSize4.ToString();

                int totalSize5 = CalculateTotal(5);
                Size_8.Text = totalSize5.ToString();

                int totalSize6 = CalculateTotal(6);
                Size_9.Text = totalSize6.ToString();

                int totalSize7 = CalculateTotal(7);
                Size_10.Text = totalSize7.ToString();

                int totalSize8 = CalculateTotal(8);
                Size_11.Text = totalSize8.ToString();

                int totalInventory = totalSize1 + totalSize2 + totalSize3 + totalSize4 + totalSize5 + totalSize6 + totalSize7 + totalSize8;
                Total_inventory_Label.Text = totalInventory.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading inventory file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // Helper method to calculate total based on column index
        private int CalculateTotal(int columnIndex)
        {
            return Inventory_ListBox.Items.Cast<string>().Sum(item => Convert.ToInt32(item.Split(',')[columnIndex]));
        }

        private void OpeningStock()
        {
            // Clear the ListBox before adding new items
            Inventory_ListBox.Items.Clear();

            // Storing and reading the file
            String inventory = "Total_Inventory.txt";
            String input = "";

            try
            {
                using (StreamReader InputFile = File.OpenText(inventory))
                {
                    while (!InputFile.EndOfStream)
                    {
                        // Reading and displaying the data
                        input = InputFile.ReadLine();
                        String[] line = input.Split(',');

                        // Adding the line to the ListBox
                        Inventory_ListBox.Items.Add($"{line[0]}, {line[1]}, {line[2]}, {line[3]}, {line[4]}, {line[5]}, {line[6]}, {line[7]}, {line[8]}");
                    }
                }

                // Calculate and display total available size stock
                int totalSize1 = CalculateTotal(1);
                Size_4.Text = totalSize1.ToString();

                int totalSize2 = CalculateTotal(2);
                Size_5.Text = totalSize2.ToString();

                int totalSize3 = CalculateTotal(3);
                Size_6.Text = totalSize3.ToString();

                int totalSize4 = CalculateTotal(4);
                Size_7.Text = totalSize4.ToString();

                int totalSize5= CalculateTotal(4);
                Size_8.Text = totalSize5.ToString();

                int totalSize6 = CalculateTotal(4);
                Size_9.Text = totalSize6.ToString();

                int totalSize7 = CalculateTotal(4);
                Size_10.Text = totalSize7.ToString();

                int totalSize8 = CalculateTotal(4);
                Size_11.Text = totalSize8.ToString();

                int totalInventory = totalSize1 + totalSize2 + totalSize3 + totalSize4 + totalSize5 + totalSize6 + totalSize7 + totalSize8;
                Total_inventory_Label.Text = totalInventory.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading inventory file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to calculate total based on column index
        private int CalculateTotal1(int columnIndex)
        {
            return Inventory_ListBox.Items.Cast<string>().Sum(item => Convert.ToInt32(item.Split(',')[columnIndex]));
        }


        public string RandomGen(int length)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();

                char[] randomString = new char[length];
                for (int i = 0; i < length; i++)
                {
                    randomString[i] = chars[random.Next(chars.Length)];
                }

                return new string(randomString);
            }

        private void Inventory_Button_Click(object sender, EventArgs e)
        {
            Inventory_Display();
            this.Sales_Report_Panel.Visible = false;
            this.Inventory_Panel.Visible = true;
        }

        private void Sales_Report_Button_Click(object sender, EventArgs e)
        {
            this.Sales_Report_Panel.Visible = true;
            this.Inventory_Panel.Visible = true;
            // Clears all existing items from the Sales Report list box
            SalesReport_ListBox.Items.Clear(); 

            try
            {
                // Gets the filename of the sales report file
                string orderInfoFile = "OrderInfo.txt";
                // Checks if the file exists
                if (File.Exists(orderInfoFile))
                {
                    // Read all lines from the OrderInfo.txt file and add them to the ListBox
                    string[] orderLines = File.ReadAllLines(orderInfoFile);
                    SalesReport_ListBox.Items.AddRange(orderLines);
                }
                else
                {
                    MessageBox.Show("Sales report file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading sales report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    }

