using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using Gvm.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Turkok.Core.Repository;
using Turkok.Model;
using Turkok.Model.Audit;
using ILogger = Serilog.ILogger;

namespace Gvm.Infra
{
    public class DataContext :  IdentityDbContext<ApplicationUser>, IDbContext
    {
        //private readonly ILogger _logger;

        public DataContext() : base("DefaultConnection")
        {
            //_logger = logger;
        }

        public IDbSet<Audit> Audits { set; get; }
        public IDbSet<GonulluVericiMerkezi> GonulluVericiMerkezleri{ get; set; }
        public IDbSet<Sehir> Sehirler { get; set; } 
        public IDbSet<Announcement> Announcements{ get; set; }  
        public IDbSet<Document> Documents{ get; set; } 
        public IDbSet<KaraListeGonulluVerici> KaraListes { get; set; }
        public IDbSet<ClaimAuthorisation> ClaimAuthorisations { get; set; }
        public IDbSet<Basvuru> Basvurular { get; set; }
        public IDbSet<GonulluVerici> GonulluVericiler { get; set; }

        public IDbSet<GonulluVericiYakini> GonulluVericiYakinis{ get; set; }
        public IDbSet<IletisimBilgileri> IletisimBilgileri { get; set; }
        public IDbSet<Hastaliklar> Hastaliklar { get; set; }
        public IDbSet<SerolojikTestler> SerolojikTestler { get; set; }
        public IDbSet<HematolojikTestler> HematolojikTestler { get; set; }
        public IDbSet<City> Cities { get; set; }
        public IDbSet<Action> Actions { get; set; }
        public IDbSet<Target> Targets { get; set; }

        public override int SaveChanges()
        {
            try
            {
                //_logger.Information("hello {foo}", "foo");
                return base.SaveChanges();
                
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    sb.AppendLine(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State));

                    
                     
                    foreach (var ve in eve.ValidationErrors)
                    {
                        string local = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);

                        sb.Append(local.ToString());
                    }
                }

                //_logger.Fatal(e, "{error}", sb.ToString());

                throw;
            }
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public static DataContext Create()
        {
            return new DataContext();
        }
    }
}