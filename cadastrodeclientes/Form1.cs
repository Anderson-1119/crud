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
using MySql.Data.MySqlClient;

namespace cadastrodeclientes
{
    public partial class frmcadastrodeclientes : Form
    {
        //conexao com o banco de dados mysql
        MySqlConnection conexao;
        string data_source = "datasource=localhost; username=root; password=; database=db_cadastro";

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

                    return;
                }

                //Cria a conexão com o banco de dados 
                conexao = new MySqlConnection(data_source);
                conexao.Open();
               // MessageBox.Show("Conexão aberta com sucesso");  teste de abertura de banco

                //Comentando SQL para inserir um novo cliente no banco de dados 
                MySqlCommand cmd = new MySqlCommand
                { Connection = conexao };
                cmd.Prepare();

                cmd.CommandText = "INSERT INTO dadosdecliente (nomecompleto, nomesocial, email, cpf)" + "VALUES (@nomecompleto, @nomesocial, @email, @cpf)";
                //Adiciona os parâmetros com os dados do formulário 
                cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@cpf", cpf);

                //Executa o comando de inserção no banco 
                cmd.ExecuteNonQuery();
                //Mensagem de sucesso 
                MessageBox.Show("Contato inserido com Sucesso: ",
                "Sucesso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }

            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL 
                MessageBox.Show("Erro" + ex.Number + "ocorreu:" + ex.Message,
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }

            catch (Exception ex)
            {
                //trata outros tipos de erro
                MessageBox.Show("ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro 
                if (conexao != null && conexao.State == ConnectionState.Open) 
                {
                    conexao.Close();
                   // MessageBox.Show("conexão fechada com sucesso");  teste
                }
            }
        }
    }
}
