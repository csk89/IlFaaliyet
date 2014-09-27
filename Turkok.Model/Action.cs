using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Turkok.Model
{
    public class Action
    {
        public int Id { get; set; }
        [DisplayName("Faaliyet Adı")]
        public string Name { get; set; }
        [DisplayName("Faaliyet No")]
        public string ActionId { get; set; }
        [DisplayName("Sorumlu Birim")] //=> Bunun icin Unit class'i olustur
        public int DeptChargedId { get; set; }
        [DisplayName("Destek Birim")] //=> Bunun icin Unit class'i olustur
        public string SupportUnit { get; set; }
        [DisplayName("İşbirliği Yapılacak Kurum ve Kuruluşlar")]
        public string Collaborators { get; set; }
        [DisplayName("Gösterge Adı")]
        public string IndicatorName { get; set; }
        [DisplayName("Gösterge Mevcut Durumu")]
        public string IndicatorCurrentState { get; set; }
        [DisplayName("2014 Yılı Gösterge Hedefi")] //Dinamik olmali
        public string IndicatorObjective { get; set; }
        [DisplayName("Gösterge Gerçekleşme Değeri")]
        public string IndicatorOccurrenceValue { get; set; }
        [DisplayName("Başlangıç Zamanı")]
        public DateTime StartDate { get; set; }
        [DisplayName("Bitiş Zamanı")]
        public DateTime EndDate { get; set; }
        [DisplayName("Açıklama")]
        public string Description { get; set; }
        public virtual Unit DeptCharged { get; set; }
        //public virtual Unit SupportUnit { get; set; }
        
    }
}
