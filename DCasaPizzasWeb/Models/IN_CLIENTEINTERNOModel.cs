using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCasaPizzasWeb.Models
{
    public class IN_CLIENTEINTERNOModel
    {
        public long ID_CLIENTEINTERNO { get; set; }
        public string DS_NOME { get; set; }
        public string CD_LOGIN { get; set; }
        public string DS_SENHA { get; set; }
        public long ID_PLANO { get; set; }
        public string DT_CADASTRO { get; set; }
        public double VL_IMPLANTACAO { get; set; }
        public int NR_PARCELASIMPLANT { get; set; }
        public string DS_CIDADE { get; set; }
        public string DS_ENDERECO { get; set; }
        public string NR_NUMERO { get; set; }
        public string CD_UF { get; set; }
        public string NR_CEP { get; set; }
        public string NR_CPF { get; set; }
        public string NR_CNPJ { get; set; }
        public string DS_TELEFONE { get; set; }
        public string DS_CELULAR { get; set; }
        public int NR_DIAVENCIMENTO { get; set; }
        public string BO_ADMIN { get; set; }
        public string DS_BAIRRO { get; set; }
    }
}
