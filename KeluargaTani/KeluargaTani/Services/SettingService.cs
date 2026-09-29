using System;
using System.Linq;
using KeluargaTani.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace KeluargaTani.Service
{
    public interface ISettingService
    {
        SettingModelVM GetSetting();
        bool UpdateSetting(SettingModelVM vm);
        Task<SettingModelVM> GetGlobalSettingsAsync();
    }

    public class SettingService : ISettingService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "GlobalAppSettings";
        public SettingService(ApplicationDbContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }
        public async Task<SettingModelVM> GetGlobalSettingsAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out SettingModelVM settings))
            {
                var dbSetting = await _db.Pengaturans.FirstOrDefaultAsync();
                
                settings = new SettingModelVM
                {
                    NamaUsaha = dbSetting.NamaPengaturan,
                    NoTelp = dbSetting.NoTelp,
                    TargetMargin = dbSetting.TargetMargin,
                    AlamatUsaha = dbSetting.AlamatUsaha, 
                };

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(2));

                _cache.Set(CacheKey, settings, cacheOptions);
            }
            return settings;
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

                _cache.Remove("GlobalAppSettings");

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

