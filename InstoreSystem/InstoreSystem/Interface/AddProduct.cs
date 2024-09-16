﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InstoreSystem.Model;
using MySql.Data.MySqlClient;

namespace InstoreSystem.Interface
{
    public partial class AddProduct : Form
    {
        public AddProduct()
        {
            InitializeComponent();
        }

        private void addNumericSize()
        {
            // Create a new Panel to hold the TextBox and NumericUpDown
            Panel rowPanel = new Panel();
            rowPanel.Size = new Size(280, 40); // Adjust the size of the panel as needed

            // TextBox to hold the size
            TextBox size = new TextBox();
            size.Size = new Size(130, 30);  // Set size of the TextBox
            size.Location = new Point(0, 5); // Set position inside the panel

            // NumericUpDown to hold the quantity
            NumericUpDown quantity = new NumericUpDown();
            quantity.Size = new Size(130, 30); // Set size of the NumericUpDown
            quantity.Location = new Point(140, 5); // Position it next to the TextBox

            // Add the controls to the panel
            rowPanel.Controls.Add(size);
            rowPanel.Controls.Add(quantity);

            // Add the panel to the FlowLayoutPanel
            pnlNumericSize.Controls.Add(rowPanel);
        }

        private void AddProduct_Load(object sender, EventArgs e)
        {
            addNumericSize();
            setNextId();
        }

        private void btnAddSize_Click(object sender, EventArgs e)
        {
            addNumericSize();
        }

        private void rbAlpha_CheckedChanged(object sender, EventArgs e)
        {
            sizingChanged();
        }

        private void rbNumeric_CheckedChanged(object sender, EventArgs e)
        {
            sizingChanged();
        }

        private void sizingChanged()
        {
            if (rbAlpha.Checked)
            {
                pnlAlphaSize.Enabled = true;
                pnlNumericSize.Enabled = false;
                btnAddSize.Enabled = false;
            }
            else if (rbNumeric.Checked)
            {
                pnlAlphaSize.Enabled = false;
                pnlNumericSize.Enabled = true;
                btnAddSize.Enabled = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                string type = "";
                string sizing = "";

                // Determine the type from checkboxes
                if (cbInstore.Checked && cbOnline.Checked)
                {
                    type = "IO";
                }
                else if (cbInstore.Checked)
                {
                    type = "I";
                }
                else if (cbOnline.Checked)
                {
                    type = "O";
                }

                // Determine the sizing from radiobuttons
                if (rbAlpha.Checked)
                {
                    sizing = "Alpha";
                }
                else if (rbNumeric.Checked)
                {
                    sizing = "Numeric";
                }

                Dictionary<string, int> quantity = new Dictionary<string, int>();

                if (rbAlpha.Checked)
                {
                    if (XS.Value > 0)
                    {
                        quantity.Add("XS", (int)XS.Value);
                    }
                    if (S.Value > 0)
                    {
                        quantity.Add("S", (int)S.Value);
                    }
                    if (M.Value > 0)
                    {
                        quantity.Add("M", (int)M.Value);
                    }
                    if (L.Value > 0)
                    {
                        quantity.Add("L", (int)L.Value);
                    }
                    if (XL.Value > 0)
                    {
                        quantity.Add("XL", (int)XL.Value);
                    }
                    if (XXL.Value > 0)
                    {
                        quantity.Add("XXL", (int)XXL.Value);
                    }
                    if (XL3.Value > 0)
                    {
                        quantity.Add("3XL", (int)XL3.Value);
                    }
                    if (XL4.Value > 0)
                    {
                        quantity.Add("4XL", (int)XL4.Value);
                    }
                }
                else if (rbNumeric.Checked)
                {
                    foreach (Control control in pnlNumericSize.Controls)
                    {
                        if (control is Panel panel)
                        {
                            string size = "";
                            int qty = 0;

                            foreach (Control childControl in control.Controls)
                            {
                                if (childControl is TextBox textBox)
                                {
                                    size = textBox.Text;
                                }
                                else if (childControl is NumericUpDown numericUpDown)
                                {
                                    qty = (int)numericUpDown.Value;
                                }
                            }

                            if (size != "" && qty > 0)
                            {
                                quantity.Add(size, qty);
                            }
                        }
                    }
                }

                Product prd = new Product(Convert.ToInt32(txtId.Text), txtName.Text, type, sizing, quantity, cmbCategory.Text, Convert.ToDecimal(txtPrice.Text), txtDescription.Text);
                prd.addProduct();

                clearFields();
                AddProduct_Load(this, null);
            }
        }

        private void setNextId()
        {
            using (MySqlConnection con = Connector.getConnection())
            {
                try
                {
                    con.Open();

                    string sql = @"SELECT MAX(productID) FROM product";

                    using (MySqlCommand com = new MySqlCommand(sql, con))
                    {
                        MySqlDataReader dr = com.ExecuteReader();

                        if (dr.Read())
                        {
                            if (dr.GetValue(0).ToString() == "")
                            {
                                txtId.Text = "1";
                            }
                            else
                            {
                                txtId.Text = (Convert.ToInt32(dr.GetValue(0)) + 1).ToString();
                            }
                        }
                        else
                        {
                            txtId.Text = "1";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool ValidateInputs()
        {
            // Validate txtName (not empty)
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Name cannot be empty!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Validate if either cbInStore or cbOnline is checked
            if (!cbInstore.Checked && !cbOnline.Checked)
            {
                MessageBox.Show("Please select either In-Store or Online!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Validate cmbCategory (text not empty)
            if (string.IsNullOrWhiteSpace(cmbCategory.Text))
            {
                MessageBox.Show("Please select a category!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Validate txtPrice (not empty and a valid number)
            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !double.TryParse(txtPrice.Text, out _))
            {
                MessageBox.Show("Please enter a valid price!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // If all validations pass
            return true;
        }

        private void clearFields()
        {
            txtName.Text = "";
            cbInstore.Checked = true;
            cbOnline.Checked = true;
            rbAlpha.Checked = true;
            pnlAlphaSize.Enabled = true;
            rbNumeric.Checked = false;
            XS.Value = 0;
            S.Value = 0;
            M.Value = 0;
            L.Value = 0;
            XL.Value = 0;
            XXL.Value = 0;
            XL3.Value = 0;
            XL4.Value = 0;
            pnlNumericSize.Controls.Clear();
            pnlNumericSize.Enabled = false;
            cmbCategory.Text = "";
            txtPrice.Text = "";
            txtDescription.Text = "";
        }
    }
}