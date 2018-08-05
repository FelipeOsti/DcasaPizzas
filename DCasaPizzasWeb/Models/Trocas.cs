using DCasaPizzasWeb.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DCasaPizzasWeb.Models
{
    public class Trocas
    {
        public long ID_TROCA { get; set; }
        public DateTime DT_TROCA { get; set; }
        private string sdsUsuario;
        private long NID_USUARIO;
        public long ID_USUARIO {
            get { return NID_USUARIO; }
            set {
                UsuarioController usuar = new UsuarioController();
                sdsUsuario = usuar.GetNomeID(value);
                NID_USUARIO = value;
            }
        }        
        public string DS_USUARIO { get { return sdsUsuario; } }
        public long NR_PONTOS { get; set; }
        public DateTime DT_CONFIRMA { get; set; }
        public DateTime DT_CANCELA { get; set; }
        public List<ItemTroca> itensTroca { get; set; }
    }

    public class ItemTroca
    {
        public long ID_TROCA { get; set; }
        public long ID_ITEMTROCA { get; set; }
        private long NID_PRODUTO;
        public long ID_PRODUTO
        {
            get { return NID_PRODUTO; }
            set
            {
                ProdutoController prodC = new ProdutoController();
                var produto = prodC.GetProduto(value);
                sdsProduto = produto.DS_PRODUTO;
                sdsTamanho = produto.DS_TAMANHO;
                NID_PRODUTO = value;
            }
        }
        public long NR_PONTOS { get; set; }
        private string sdsProduto;
        private string sdsTamanho;
        public string DS_PRODUTO { get { return sdsProduto; } }
        public string DS_TAMANHO { get { return sdsTamanho; } }
    }
}