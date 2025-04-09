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

            //Configuração inicial do ListView para exibição dos dados dos clientes 
            lstCliente.View = View.Details;
            lstCliente.LabelEdit = true;
            lstCliente.AllowColumnReorder = true;
            lstCliente.FullRowSelect = true;
            lstCliente.GridLines = true;
            //Define a visualização como "Detalhes" 
            //Permite editar os títulos das colunas 
            //Permite reordenar as colunas 
            //Seleciona a linha inteira ao clicar 
            //Exibe as linhas de grade no ListView 
            //Definindo as colunas do ListView 
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left); //Coluna de código 
            lstCliente.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna de Nome Completo 
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left); //Coluna de Nome Social 
            lstCliente.Columns.Add("E-mail", 200, HorizontalAlignment.Left); //Coluna de E-mail 
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left); //Coluna de CPF

            //carregar os dados do cliente na interface
            carregar_clientes();
        }

        private void carregar_clientes_com_query(string query)
        {
            try
            {
                //Cria a conexão com o banco de dados 
                conexao = new MySqlConnection(data_source);
                conexao.Open();

                //Executa a consulta SQL fornecida 
                MySqlCommand cmd = new MySqlCommand(query, conexao);
                //Se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa 
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }
                //Executa o comando e obtém os resultados 
                MySqlDataReader reader = cmd.ExecuteReader();
                //Limpa os itens existentes no ListView antes de adicionar novos 
                lstCliente.Items.Clear();
                //Preesh //Pree she o ListView com os dados dos clientes 
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta 
                    string[] row =
                    {
                     Convert.ToString(reader.GetInt32(0)), //Codigo 
                           reader.GetString(1),
                           reader.GetString(2),
                           reader.GetString(3),
                           reader.GetString(4)
                    };
                    //Adiciona a linha ao ListView 
                    lstCliente.Items.Add(new ListViewItem(row));
                }
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

        //Método para carregar todos os clientes no ListView (usando uma consulta sem parâmetros) 
         private void carregar_clientes()
        {
            string query = "SELECT * FROM dadosdecliente ORDER BY codigo DESC";
            carregar_clientes_com_query(query);
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

                //Limpa os campos no após o sucesso 
                txtNomeCompleto.Text = String.Empty;
                txtNomeSocial.Text = ""; 
                txtEmail.Text = ""; 
                txtCPF.Text = "";

                //carregar os clientes na list view 
                carregar_clientes();

                //muda para aba de pesquisa
                tabControl1.SelectedIndex = 1;
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

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
        string query = "SELECT * FROM dadosdecliente WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER BY codigo DESC";
        carregar_clientes_com_query(query);
        }
    }
}
