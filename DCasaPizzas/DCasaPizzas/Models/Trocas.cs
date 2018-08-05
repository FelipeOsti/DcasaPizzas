using System;
using System.Collections.Generic;
using System.Text;

namespace DCasaPizzas.Models
{
    class Trocas
    {
        public int ID_TROCA { get; set; }
        public DateTime DT_TROCA { get; set; }
        public int ID_USUARIO { get; set; }
        public int NR_PONTOS { get; set; }
        public DateTime DT_CONFIRMA { get; set; }
        public List<ItemTroca> itensTroca { get; set; }
    }

    class ItemTroca
    {
        public int ID_ITEMTROCA { get; set; }
        public int ID_PRODUTO { get; set; }
        public int NR_PONTOS { get; set; }
        public string DS_PRODUTO { get; set; }
        public string DS_TAMANHO { get; set; }
    }
}
