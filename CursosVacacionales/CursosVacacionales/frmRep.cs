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
    public partial class frmRep : Form
    {
        linqCursosDataContext db = new linqCursosDataContext();
        public frmRep()
        {
            InitializeComponent();
        }

        public frmRep(int curso)
        {
            InitializeComponent();
            
            var resultado = from C in db.CURSOS
                            from A in db.ALUMNOS
                            from Mat in db.MATERIAS
                            from Maest in db.MAESTROS
                            from CA in db.CURSOALUMNOS

                            where C.curso_id == curso
                            && A.alumno_id == CA.alumno_id
                            && C.curso_id == CA.curso_id
                            && C.maestro_id == Maest.maestro_id
                            && C.materia_id == Mat.materia_id
                            
                            select new
                            {
                                C.curso_id,
                                Mat.materia_nombre,
                                C.curso_hora,
                                C.curso_salon,
                                Maest.maestro_nombre,
                                A.alumno_id,
                                A.alumno_nombre,
                                A.alumno_adeudo
                            };
            reporteCurso rpt = new reporteCurso();
            rpt.SetDataSource(resultado);
            crystalReportViewer1.ReportSource = rpt;
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
