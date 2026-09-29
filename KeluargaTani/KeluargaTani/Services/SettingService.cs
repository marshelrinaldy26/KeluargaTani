using System;
using System.Linq;
using KeluargaTani.Models;

namespace KeluargaTani.Service
{
    public class SettingModelVM
    {
        public string NamaUsaha { get; set; }
        public string NoTelp { get; set; }
        public int TargetMargin { get; set; }
        public string AlamatUsaha { get; set; }
    }

    public interface ISettingService
    {
        SettingModelVM GetSetting();
        bool UpdateSetting(SettingModelVM vm);
    }

    public class SettingService : ISettingService
    {
        private readonly ApplicationDbContext _db;
        public SettingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public SettingModelVM GetSetting()
        {
            var setting = _db.Pengaturans.FirstOrDefault();
            if (setting == null) return new SettingModelVM();

            return new SettingModelVM
            {
                NamaUsaha = setting.NamaPengaturan,
                NoTelp = setting.NoTelp,
                TargetMargin = setting.TargetMargin,
                AlamatUsaha = setting.AlamatUsaha
            };
        }

        public bool UpdateSetting(SettingModelVM vm)
        {
            try
            {
                var setting = _db.Pengaturans.FirstOrDefault();
                if (setting == null)
                {
                    setting = new Pengaturan();
                    _db.Pengaturans.Add(setting);
                }

                setting.NamaPengaturan = vm.NamaUsaha ?? "-";
                setting.NoTelp = vm.NoTelp ?? "-";
                setting.TargetMargin = vm.TargetMargin;
                setting.AlamatUsaha = vm.AlamatUsaha;

                _db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
