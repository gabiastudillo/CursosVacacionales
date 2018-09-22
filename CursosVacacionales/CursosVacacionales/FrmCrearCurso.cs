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
    public partial class FrmCrearCurso : Form
    {
        linqCursosDataContext bd = new linqCursosDataContext();
        public FrmCrearCurso()
        {
            InitializeComponent();
            var query_where1 = from a in bd.MATERIAS select a;
            foreach (var a in query_where1)
            {
                cbomateria.Items.Add(a.materia_nombre);
            }
            var query_where2 = from b in bd.MAESTROS select b;
            foreach (var b in query_where2)
            {
                cbomaestro.Items.Add(b.maestro_nombre);
            }
           
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FrmCrearCurso_Load(object sender, EventArgs e)
        {
            var innerJoinQuery = from cu in bd.CURSOS
                                 join mat in bd.MATERIAS on cu.materia_id equals mat.materia_id
                                 join mae in bd.MAESTROS on cu.maestro_id equals mae.maestro_id                                
                                 select new { Curso = cu.curso_id, Materia = mat.materia_nombre,
                                              Maestro = mae.maestro_nombre, Horario = cu.curso_hora,
                                              Salón = cu.curso_salon }; 
            dataGridView2.DataSource = innerJoinQuery.ToList();
        }

        public void Guardar()
        {            
            try
            {
                var m = from a in bd.MAESTROS
                        where a.maestro_nombre == cbomaestro.SelectedItem.ToString() select a;
                MAESTROS mae = m.First();
                string id_maestro = mae.maestro_id;

                var mat = from a in bd.MATERIAS
                          where a.materia_nombre == cbomateria.SelectedItem.ToString() select a;
                MATERIAS mate = mat.First();
                int id_materia = mate.materia_id;

                CURSOS C = new CURSOS();

                C.maestro_id = id_maestro;
                C.materia_id = id_materia;
                C.curso_hora = cbohora.SelectedItem.ToString();
                C.curso_salon = cbosalon.SelectedItem.ToString();                

                bd.CURSOS.InsertOnSubmit(C);
                bd.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (validarmaestro() == false)
            {
                if (validarsalon() == false)
                {
                    Guardar();
                    MessageBox.Show("CURSO CREADO");

                    var innerJoinQuery = from cu in bd.CURSOS
                                         join mat in bd.MATERIAS on cu.materia_id equals mat.materia_id
                                         join mae in bd.MAESTROS on cu.maestro_id equals mae.maestro_id
                                         select new { Curso = cu.curso_id, Materia = mat.materia_nombre,
                                             Maestro = mae.maestro_nombre, Horario = cu.curso_hora,
                                             Salón = cu.curso_salon }; 
                    dataGridView2.DataSource = innerJoinQuery.ToList();
                    cbohora.SelectedItem = "";
                    cbosalon.SelectedItem = "";
                    cbomateria.SelectedItem = "";
                    cbomaestro.SelectedItem = "";
                }
                else
                {
                    MessageBox.Show("El salón ya está ocupado en el horario seleccionado");
                }

            }
            else
            {
                MessageBox.Show("Maestro ya tiene asignado ese horario");
            }
        }


        public bool validarmaestro()
        {
            var m = from a in bd.MAESTROS where a.maestro_nombre == cbomaestro.SelectedItem.ToString() select a;
            MAESTROS mae = m.First();
            string id = mae.maestro_id;
            int linq = bd.CURSOS.Where
             (c => c.maestro_id == id && c.curso_hora == cbohora.SelectedItem.ToString()).Count();
            if (linq > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool validarsalon()
        {
            
            int linq = bd.CURSOS.Where
             (c => c.curso_salon == cbosalon.SelectedItem.ToString() && 
             c.curso_hora == cbohora.SelectedItem.ToString()).Count();
            if (linq > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnMatricular_Click(object sender, EventArgs e)
        {
            if (txtced.Text=="")
            {
                MessageBox.Show("Debe ingresar la cédula del alumno");
                return;
            }
            if (txtnom.Text=="")
            {
                MessageBox.Show("Debe ingresar el nombre del alumno");
                return;
            }
            if (txtadeudo.Text == "")
            {
                MessageBox.Show("Debe seleccionar al menos un curso a matricular");
                return;
            }
            try
            {
                var alu = from al in bd.ALUMNOS where al.alumno_id == txtced.Text select al;
                if (alu.Count()>0)
                {
                    MessageBox.Show("El alumno ya está matriculado");
                    return;
                }
                ALUMNOS a = new ALUMNOS();
                a.alumno_id = txtced.Text;
                a.alumno_nombre = txtnom.Text;
                a.alumno_adeudo = Decimal.Parse(txtadeudo.Text);
                bd.ALUMNOS.InsertOnSubmit(a);

                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    CURSOALUMNOS ca = new CURSOALUMNOS();
                    ca.alumno_id = txtced.Text;
                    ca.curso_id = Convert.ToInt16(dataGridView1.Rows[i].Cells[0].Value);
                    bd.CURSOALUMNOS.InsertOnSubmit(ca);
                }

                    bd.SubmitChanges();
                MessageBox.Show("Se registró la matricula");
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnAgregarCurso_Click(object sender, EventArgs e)
        {
            Matricular f = new Matricular();
            f.pasadoPro += new Matricular.pasarPro(ejecutarPro);
            f.Show();
        }


        public void ejecutarPro(int cur, string mat, string mae, string hor, string sal)
        {
            int bandera = 0;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                int id_curso = Convert.ToInt32(dataGridView1.Rows[i].Cells[0].Value);
                string horario = dataGridView1.Rows[i].Cells[3].Value.ToString();
                if (id_curso == cur)
                {
                    bandera = 1;
                    MessageBox.Show("Ya agregó el curso de "+mat+" con el maestro "+mae+" "+hor);
                    return;
                }
                if (horario == hor)
                {
                    bandera = 1;
                    MessageBox.Show("Ya agregó un curso en el horario "+hor);
                    return;
                }
            }
            double adeudo;
            if (txtadeudo.Text == "")
            {
                adeudo = 0.00;
            }
            else
            {
                adeudo = double.Parse(txtadeudo.Text);
            }
            adeudo = adeudo + 10.00;            
            if (adeudo>40)
            {
                MessageBox.Show("Ya no se puede agregar cursos debido a que no se puede " +
                    "superar un adeudo de $ 40,00");
            }
            else
            {
               if (bandera == 0)
                {
                    dataGridView1.Rows.Insert(0, cur, mat, mae, hor, sal);
                    txtadeudo.Text = adeudo.ToString();
                }
            }

        }

        private void btnGenRep_Click(object sender, EventArgs e)
        {
            int curso = Convert.ToInt32(dataGridView2[0, dataGridView2.CurrentCell.RowIndex].Value);
            frmRep fr = new frmRep(curso);
            fr.Show();
        }

        private void cbomaestro_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
