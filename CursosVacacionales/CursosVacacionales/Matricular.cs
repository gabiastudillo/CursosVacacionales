using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CursosVacacionales
{
    public partial class Matricular : Form
    {
        public delegate void pasarPro(int curso, string materia, string maestro, string horario, string salon);
        public event pasarPro pasadoPro;
        linqCursosDataContext bd = new linqCursosDataContext();
        public Matricular()
        {
            InitializeComponent();
            
        }

        private void btnMatricular_Click(object sender, EventArgs e)
        {

        }

        private void Matricular_Load(object sender, EventArgs e)
        {
            var innerJoinQuery =
            from cu in bd.CURSOS
            join mat in bd.MATERIAS on cu.materia_id equals mat.materia_id
            join mae in bd.MAESTROS on cu.maestro_id equals mae.maestro_id
            //where ma.curso == comboBox2.SelectedItem.ToString()
            select new { Curso= cu.curso_id, Materia=mat.materia_nombre,Maestro=mae.maestro_nombre, Horario=cu.curso_hora, Salón=cu.curso_salon }; //produces flat sequence

            dataGridView1.DataSource = innerJoinQuery.ToList();

        }

        private void btnAgregarCurso_Click(object sender, EventArgs e)
        {
                if (dataGridView1.CurrentCell.RowIndex >= 0)
                {

                int curso=Convert.ToInt32(dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value);
                string materia =dataGridView1[1, dataGridView1.CurrentCell.RowIndex].Value.ToString();
                string maestro = dataGridView1[2, dataGridView1.CurrentCell.RowIndex].Value.ToString();
                string horario =dataGridView1[3, dataGridView1.CurrentCell.RowIndex].Value.ToString();
                string salon= dataGridView1[4, dataGridView1.CurrentCell.RowIndex].Value.ToString();


                pasadoPro(curso, materia, maestro, horario, salon);
                    } //produces flat sequence
            
                else
                {
                    MessageBox.Show("Debe seleccionar un curso");
                }
            
        }
    }
}
