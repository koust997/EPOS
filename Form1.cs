using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace EPOS_System
{
    public partial class PrimeTrend_Shoes : Form
    {
        // Delared an Array containing product names
        public static string[] Product_Name { get; private set; } = { "Nike Air Jordan", "Adidas Superstar", "Puma Suede", "Reebok Classic", "Crocs Classic Clog", "Converse Chuck Taylor All-Star", "Vans Old Skool", "Under Armour Curry", "ASICS Gel-Kayano", "Skechers Go Walk" };
        // Delared an Array containing product Sizes
        static readonly string[] Sizes = { "4", "5", "6", "7", "8", "9", "10", "11" };
        // Delared an Array containing product Costs
        static readonly int[] Product_Cost = { 50, 45, 40, 42, 47, 55, 60, 70, 67, 72 };

        //Declaring Shoes inventory as 2-D array 
        static readonly int[,] Inventory = {
                    { 35, 37, 33, 39, 40, 35, 35, 35 },
                    { 53, 59, 53, 53, 70, 53, 53, 61 },
                    { 47, 30, 47, 47, 47, 37, 47, 47 },
                    { 31, 31, 36, 31, 31, 30, 31, 31 },
                    { 40, 38, 40, 40, 40, 40, 42, 40 },
                    { 44, 46, 44, 43, 42, 44, 44, 44 },
                    { 29, 29, 27, 29, 29, 29, 22, 29 },
                    { 38, 35, 38, 38, 38, 38, 42, 38 },
                    { 32, 32, 32, 30, 32, 32, 34, 32 },
                    { 36, 33, 36, 36, 36, 36, 29, 36 },
                };
        // get: Allows the retrieval of the Inventory_Path value from outside the class.private set: Restricts external modification of Inventory_Path.It can only be set within the class itself.
        public static string Inventory_Path { get; private set; } = "Total_Inventory.txt";
        public static string Order_Details_Path { get; private set; } = "OrderInfo.txt";

        //Assigning number of rows to Rows variable
        public static int Rows { get; private set; } = 10;
        //Assigning number of rows to Columns variable
        public static int Cols { get; private set; } = 8;
        public string[,] temp = new String[10, 8];
        //Declaring Variables related to the selected shoes and cost
        public string Shoes_Selected;
        public decimal Total_Cost;
        public decimal ItemCost;
        public int Shoes_Quantity = 0;
        public int Size_Selected;
        public decimal TotalCartCost;
        public int quantity = 0;
        public decimal CartCost;
        public int Selected_Quantity;
        public int Total_Quantity;
        public int[,] InitialInventoryListOfItems { get; private set; } = new int[Rows, Cols];

        //Declaring constants
        const int LENGTH = 8, CUSTOMIZED = 5;

        // Constructor for the PrimeTrend_Shoes class
        public PrimeTrend_Shoes()
        {
            // Initializing the form
            InitializeComponent();
            this.Text = "PrimeTrend Shoes POS";
            Cart_ListBox.Visible = false;
            Search_GroupBox.Visible = false;
            Clear_Button.Enabled = false;

            // Checking if the inventory file exists and is empty
            if (File.Exists(Inventory_Path) && new FileInfo(Inventory_Path).Length == 0)
            {
                // If the inventory file is empty, create a new file and display a message
                WriteToFile();
                MessageBox.Show("Inventory File doesn't exist or is empty. Created a new Inventory File.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // If the inventory file exists and is not empty, read the inventory data   
                InventoryRead();
            }
            InventoryRead();
        }
        // Creating an instance of the Reports class
        Reports PrimeTrendShoes = new Reports();

        private void Add_Button_Click(object sender, EventArgs e)
        {
            Checkout_Panel.Visible = true;
            // Check if product and size are selected
            if (Select_Shoes_List_Box.SelectedIndex == -1 || Select_Size_ListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a product and size.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // Enable necessary buttons and components for cart functionality
            Clear_Button.Enabled = true;
            Cart_ListBox.Enabled = true;
            Cart_ListBox.Visible = true;
            Purchase_Button.Enabled = true;

            //getting value and storing to variable from user selected Numerical updown button
            int Shoes_Quantity = (int)Select_Quantity_UpDown.Value;

            //Checking and alerting customer, if the customer has not selected shoes quantity
            if (Shoes_Quantity <= 0)
            {
                MessageBox.Show("Please select at least 1 quantity.", "Quantity Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            string selectedShoes = $"{Select_Shoes_List_Box.SelectedItem} (Size {Sizes[Select_Size_ListBox.SelectedIndex]})";
            Cart_ListBox.Items.Add(selectedShoes);

            // Getting the indices of the selected product and size
            int Product_Index = Select_Shoes_List_Box.SelectedIndex;
            int Size_Index = Select_Size_ListBox.SelectedIndex;

            Total_Quantity += Shoes_Quantity;
            Quantity_ListBox.Items.Add(Shoes_Quantity.ToString());

            // Calculate the cost for the selected item
            ItemCost = Product_Cost[Product_Index];
            Size_Selected = Size_Index;
            CartCost = ItemCost * Shoes_Quantity;
            Total_Cost += CartCost;
            Total_Cost_TextBox.Text = Total_Cost.ToString("C2");
            Cart_Cost_TextBox.Text = TotalCartCost.ToString("C2");

            // Checking available quantity in stock for the selected product and size
            int QuantityInStock = InitialInventoryListOfItems[Select_Shoes_List_Box.SelectedIndex, Select_Size_ListBox.SelectedIndex];
            int Avail_Quantity;
            // If the selected quantity exceeds available stock
            if (Shoes_Quantity > QuantityInStock)
            {
                Total_Cost = 0;
                Reset();
                Remove_Button.Enabled = true;
                Add_Button.Enabled = true;
                Purchase_Button.Enabled = false;
                MessageBox.Show($"Unfortunately, only {QuantityInStock} quantity is available.", "Stock is not available", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            //Checking available quantity in stock for the selected product and size also Update available quantity, cart cost, and inventory if items are available
            else
            {
                Avail_Quantity = QuantityInStock - Shoes_Quantity;
                temp[Product_Index, Size_Index] = Avail_Quantity.ToString();
                TotalCartCost += CartCost; // Add cost only when the item is available
                Cart_Cost_TextBox.Text = TotalCartCost.ToString("C2");
                Inventory[Product_Index, Size_Index] -= Shoes_Quantity;
            }

            Select_Quantity_UpDown.Value = 0;
        }



        private void Select_Quantity_UpDown_ValueChanged(object sender, EventArgs e)
        {
            // Get the current quantity selected
            Shoes_Quantity = (int)Select_Quantity_UpDown.Value;

            // Check if an item is selected in both list boxes
            if (Select_Shoes_List_Box.SelectedIndex != -1 && Select_Size_ListBox.SelectedIndex != -1)
            {
                // Check if the selected index is within the bounds of the arrays
                if (Select_Shoes_List_Box.SelectedIndex < Product_Cost.Length && Select_Size_ListBox.SelectedIndex < Sizes.Length)
                {
                    // Calculate item cost based on the selected shoe and size
                    ItemCost = Product_Cost[Select_Shoes_List_Box.SelectedIndex];
                    Size_Selected = Select_Size_ListBox.SelectedIndex;
                    CartCost = ItemCost * Shoes_Quantity;

                    if (Customized_Grafitti_CheckBox.Checked)
                    {
                        // Add customization cost to the total cart cost if checked
                        decimal Customized_Cost = CUSTOMIZED * Shoes_Quantity;
                        TotalCartCost = CartCost + Customized_Cost;
                    }
                    else
                    {
                        // Otherwise, set the total cart cost to the item cost multiplied by quantity
                        TotalCartCost = CartCost;
                    }

                    // Update the displayed cart cost
                    Cart_Cost_TextBox.Text = TotalCartCost.ToString("C2");
                }
                else
                {
                    // Handle the case where the selected index is out of range
                    MessageBox.Show("Selected index is out of range.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Handle the case where an item is not selected in one or both list boxes
                MessageBox.Show("Please select an item in both list boxes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Quantity_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Quantity_ListBox.Items.Add(Shoes_Quantity.ToString());
            //Cart_ListBox.Items.Add(Select_Shoes_List_Box.SelectedItem)
        }

        private void Select_Size_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        //Creating method to write into text file
        private void WriteToFile()
        {
            try
            {
                // Opening a StreamWriter to write to the inventory file
                using (StreamWriter writer = new StreamWriter(Inventory_Path))
                {
                    // Iterating through each row in the inventory
                    for (int row = 0; row < Rows; row++)
                    {
                        // Iterating through each column in the inventory
                        for (int col = 0; col < Cols; col++)
                        {
                            // Writing the inventory value to the file
                            writer.Write(Inventory[row, col]);

                            // Adding a comma after each value except the last one in a row
                            if (col < Cols - 1)
                            {
                                writer.Write(",");
                            }
                        }

                        // Moving to the next line after writing all column values in a row
                        writer.WriteLine();
                    }
                }

                // Showing a success message after writing the inventory to the file
                MessageBox.Show("Inventory successfully saved to file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Showing an error message if there's an issue writing the file
                MessageBox.Show($"Error writing inventory file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void Clear_Button_Click(object sender, EventArgs e)
        {
            Reset();
            Clear_Button.Enabled = false;
            Remove_Button.Enabled = false;
            Add_Button.Enabled = true;
        }

        private void Remove_Button_Click(object sender, EventArgs e)
        {
            Add_Button.Enabled = true;
            try
            {
                // Check if any item is selected in the Quantity_ListBox
                if (Cart_ListBox.SelectedIndex != -1)
                {
                    int SelectedIndex = Cart_ListBox.SelectedIndex;
                    int selectedQuantity = int.Parse(Quantity_ListBox.Items[SelectedIndex].ToString());

                    Total_Cost -= ((Product_Cost[Cart_ListBox.SelectedIndex]) * selectedQuantity);
                    Total_Cost_TextBox.Text = Total_Cost.ToString("C");
                    int Selected_Index = Cart_ListBox.SelectedIndex;
                    // Getting the selected item from the list box
                    string Selected_Item = Cart_ListBox.SelectedItem.ToString();

                    // Removing the selected item from the list box
                    Cart_ListBox.Items.Remove(Selected_Item);
                    Quantity_ListBox.Items.RemoveAt(Selected_Index);

                    // Showing the messagebox and letting know the customer of the succssfull removal of item
                    MessageBox.Show("Item Removed Successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (Cart_ListBox.Items.Count == 0) // Check if the cart is empty
                    {
                        Total_Cost = 0; // Reset the total cost to zero
                        Total_Cost_TextBox.Text = Total_Cost.ToString("C");
                    }
                }
                else
                {
                    MessageBox.Show("Please select an item to remove.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
          
        private void Reset()
        {
            foreach (var index in Select_Shoes_List_Box.SelectedIndices)
            {
                // Clear selections in Select_Shoes_List_Box
                Select_Shoes_List_Box.ClearSelected();

                // Clear selections in Select_Size_ListBox
                Select_Size_ListBox.ClearSelected();
            }
            
            Cart_ListBox.Items.Clear();
            Quantity_ListBox.Items.Clear();
            Add_Button.Enabled = false;
            Clear_Button.Enabled = false;
            Customized_Grafitti_CheckBox.Enabled = false;
        }

        private void Reports_Button_Click(object sender, EventArgs e)
        {
            Reset();
            Reports ReportForm = new Reports();
            ReportForm.ShowDialog();
            ReportForm.Focus();
        }

        public void InventoryRead()
        {
            bool Check_InvFile = false;

            try
            {
                if (new FileInfo(Inventory_Path).Length == 0)
                {
                    WriteToFile();
                    MessageBox.Show("Inventory File was empty. Created a new Inventory File.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Read all lines from the inventory file
                string[] lines = File.ReadAllLines(Inventory_Path);

                // Loop through each line
                for (int row = 0; row < lines.Length && row < Rows; row++)
                {
                    // Split the line into values
                    string[] values = lines[row].Split(',');

                    // Loop through each value and fill the inventory array
                    for (int col = 0; col < values.Length && col < Cols; col++)
                    {
                        if (int.TryParse(values[col], out int inventoryValue))
                        {
                            InitialInventoryListOfItems[row, col] = inventoryValue;
                        }
                        else
                        {
                            // Checking if WriteToFile() has already been called
                            if (!Check_InvFile) 
                            {
                                //Writing to file in case inventory file is missing
                                WriteToFile();
                                // Set the flag to true after calling WriteToFile()
                                Check_InvFile = true; 
                            }
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading inventory file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Storing order details
        public void StoreInFile()
    {
        String order = Order_Details_Path;


        using (StreamWriter OrderFile = new StreamWriter(order, append: true))
        {
            String Date = DateTime.UtcNow.ToString("dd/MM/yyyy");
            String Order_Id = Order_ID_TextBox.Text;

            for (int i = 0; i < Cart_ListBox.Items.Count; i++)
            {
                string entry = "";
                string listItem = Cart_ListBox.Items[i].ToString();

                string[] parts = listItem.Split(',');
                for (int j = 0; j < Math.Min(5, parts.Length); j++)
                {
                    entry += parts[j] + ",";
                }

                entry = Order_Id + "," + entry + Date;
                OrderFile.WriteLine(entry.Trim(','));
            }
        } // The using statement will automatically close the StreamWriter

        // Proceeding with updating the inventory file
        File.Delete(Inventory_Path);

        // using statement for OutputFile the file
        using (StreamWriter OutputFile = File.CreateText(Inventory_Path))
        {
            for (int i = 0; i < Rows; i++)
            {
                String line = string.Join(",", Enumerable.Range(0, 5)
                .Select(j => temp[i, j]?.Trim()));
                // The using statement will automatically close the StreamWriter
                OutputFile.WriteLine(line);
            }
        } 
    }


        private void Purchase_Button_Click(object sender, EventArgs e)
        {
            Selection_Panel.Visible = false;
            // Showing a confirmation dialog
            DialogResult result = MessageBox.Show("Are you sure you want to purchase?", "Confirm Purchase", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // If the customer confirms, generate a random order ID
                string Random_AlphaNum = PrimeTrendShoes.RandomGen(LENGTH);
                Order_ID_TextBox.Text = Random_AlphaNum;

                // Performing other actions related to the purchase, such as storing the order details
                StoreInFile();
                Reset();
                Search_GroupBox.Visible = true;
            }
            else
            {
                // If the customer clicks "No" in the confirmation dialogue
                Reset();
            }
            
        }

        private void PerformSearch(SearchBy searchBy)
        {
            Search_ListBox.Items.Clear(); // Clear previous search results
            List<string> SearchOrderDetails = new List<string>();

            string searchTermID = Searchby_ID_TextBox.Text;
            string searchTermDate = Date_TimePicker.Value.ToString("yyyy-MM-dd");

            try
            {
                using (StreamReader OrderDetailsSearch = new StreamReader(Order_Details_Path))
                {
                    while (!OrderDetailsSearch.EndOfStream)
                    {
                        string line = OrderDetailsSearch.ReadLine();
                        string[] TransactionItems = line.Split(',');

                        if ((searchBy == SearchBy.ID || searchBy == SearchBy.Both) && TransactionItems[0] == searchTermID)
                        {
                            SearchOrderDetails.Add(line);
                        }
                        else if ((searchBy == SearchBy.Date || searchBy == SearchBy.Both) && TransactionItems[1] == searchTermDate)
                        {
                            SearchOrderDetails.Add(line);
                        }
                    }
                }

                if (SearchOrderDetails.Count == 0)
                {
                    MessageBox.Show("No results found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (string orderDetail in SearchOrderDetails)
                {
                    Search_ListBox.Items.Add(orderDetail);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Search_By_ID_Button_Click(object sender, EventArgs e)
        {
            PerformSearch(SearchBy.ID);
        }

        private void Search_ByDate_Button_Click(object sender, EventArgs e)
        {
            PerformSearch(SearchBy.Date);
        }

        private void Search_Button_Click(object sender, EventArgs e)
        {
            PerformSearch(SearchBy.Both);
        }

        private enum SearchBy
        {
            ID,
            Date,
            Both
        }


        private void Customized_Grafitti_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Customized_Price_Label.Visible = true;
        }

        private void Clear_All_Button_Click(object sender, EventArgs e)
        {
            //Calling the reset method
            Reset();
            // Clear the search box text
            Searchby_ID_TextBox.Text = "";

            // Clear the search list box
            Search_ListBox.Items.Clear();
        }

        private void Exit_Button_Click(object sender, EventArgs e)
        {
            //Closing the form after clicking Exit button
            this.Close();
        }
    }
}
    