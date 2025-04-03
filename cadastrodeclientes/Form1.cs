using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions; 

namespace cadastrodeclientes
{
    public partial class frmcadastrodeclientes : Form
    {
        public frmcadastrodeclientes()
        {
            InitializeComponent();
        }
        //validação regex
        private bool isValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
        
        //funcao para validar se o CPF tem 11 digitos 
        private bool isValidCPFLegth (string cpf)
        {
            //remover quaisquer caracteres nao numericos 
            cpf = cpf.Replace(".", "").Replace("-", "");

            // verificar se o cpf tem 11 digitos
            if (cpf.Length !=11 || !cpf.All(char.IsDigit))
            {
                return false;
            }
            return true;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {    //validaçao de campos obrigatorios
                if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim()) ||
                    string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                    string.IsNullOrEmpty(txtCPF.Text.Trim()))
            {
                    MessageBox.Show("todos os campos devem ser preenchidos", "validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; 
                }
                //validaçao de email
                string email = txtEmail.Text.Trim();
                if (!isValidEmail(email))
                {
                    MessageBox.Show("E-mail invalido, certifique-se se o e-mail esta correto", "Validação",
                         MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
                //validaçao do cpf
                string cpf = txtCPF.Text.Trim();
                if (!isValidCPFLegth(cpf))
                {
                    MessageBox.Show("CPF invalido, verifique se os 11 digitos estão corretos ", "Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);                   
                }
                return;
            }
            catch (Exception ex)
            {
                //trata outros tipos de erro
                MessageBox.Show("ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
