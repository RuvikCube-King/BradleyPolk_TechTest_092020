using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DnDBeyondBusiness;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace DnDBeyondApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DndTechTestController : Controller
    {
        private IMemoryCache _cache;
        public DndTechTestController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        // HTTPGET
        // Retrieves character data for Briv from the memory cache.  If data isn't loaded 
        // into the Memory Cache, loads the data from briv.json.  Returns a json serialized
        // string of the Character class object.
        [HttpGet]
        public string Get()
        {
            var _briv = new Character();

            if (!_cache.TryGetValue("briv", out _briv))
            {
                _briv = Character.LoadFromFile();

                _cache.Set("briv", _briv);
            }
            return JsonConvert.SerializeObject(_briv);

        }

        // HTTPPOST
        // Adds temporary Hp to the loaded character Briv.  If the character already has
        // temporary Hp and the value passed is greater than the character's current temp
        // hp, sets the character's temp hp to the passed in value.  If the value passed is
        // lower than the character's current temp hp, nothing will occur. If the value is
        // negative, will pass an error.
        // PARAMETERS
        //   -tempValue, type int, the amount of temporary hp to give to the character.
        [HttpPost]
        [Route("addTemp")]
        public IActionResult AddTemp(int tempValue)
        {
            if (tempValue >= 0)
            {
                var _briv = (Character)_cache.Get("briv");

                _briv.AddTempHp(tempValue);

                _cache.Set("briv", _briv);

                return RedirectToAction("Get");
            }
            else {
                return StatusCode(500, "The value you wish to assign as temporary hp must be greater than or equal to 0.");
            }
        }

        // HTTPOST
        // Heals damage taken by loaded character Briv.  If amount of healing would put the
        // character over their Max Hp, sets their current HP to Max.  If the amount of healing
        // passed is negative, will pass an error.
        // PARAMETERS
        //   -healValue, type int, the amount of hp to heal.
        [HttpPost]
        [Route("heal")]
        public IActionResult Heal(int healValue)
        {
            if (healValue >= 0)
            {
                var _briv = (Character)_cache.Get("briv");

                _briv.Heal(healValue);

                _cache.Set("briv", _briv);

                return RedirectToAction("Get");
            } else
            {
                return StatusCode(500, "The value you wish to heal must be expressed as a positive number.");
            }
            
        }

        // HTTPOST
        // Deals damage to the loaded in character Briv.  If the character has resistance
        // or vulnerability to the submitted damage type, it will calculate damage appropriately
        // PARAMETERS:
        //   -damageType, type string, the name of the damage type dealt
        //   -damage, type int, the amount of damage dealt.
        //
        // If the damageType submitted doesn't match any of the damage types covered in the enum
        // DamageTypes, this call will return a 500 error.
        [HttpPost]
        [Route("attack")]
        public IActionResult Attack(string damageType, int damage)
        {
            var _briv = (Character)_cache.Get("briv");

            DamageTypes type = DamageTypes.Error;
            switch (damageType.ToLower())
            {
                case "bludgeoning":
                    type = DamageTypes.Bludgeoning;
                    break;
                case "piercing":
                    type = DamageTypes.Piercing;
                    break;
                case "slashing":
                    type = DamageTypes.Slashing;
                    break;
                case "magicalbludgeoning":
                    type = DamageTypes.MagicalBludgeoning;
                    break;
                case "magicalpiercing":
                    type = DamageTypes.MagicalPiercing;
                    break;
                case "magicalslashing":
                    type = DamageTypes.MagicalSlashing;
                    break;
                case "fire":
                    type = DamageTypes.Fire;
                    break;
                case "cold":
                    type = DamageTypes.Cold;
                    break;
                case "lightning":
                    type = DamageTypes.Lightning;
                    break;
                case "thunder":
                    type = DamageTypes.Thunder;
                    break;
                case "force":
                    type = DamageTypes.Force;
                    break;
                case "acid":
                    type = DamageTypes.Acid;
                    break;
                case "poison":
                    type = DamageTypes.Poison;
                    break;
                case "radiant":
                    type = DamageTypes.Radiant;
                    break;
                case "necrotic":
                    type = DamageTypes.Necrotic;
                    break;
                case "psychic":
                    type = DamageTypes.Psychic;
                    break;
                default:
                    type = DamageTypes.Error;
                    break;
            }

            if (type != DamageTypes.Error)
            {
                _briv.TakeDamage(type, damage);

                _cache.Set("briv", _briv);


                return RedirectToAction("Get");
            } else
            {
                return StatusCode(500, "Damage Type was not a valid damage type, submit a valid damage type or check the spelling of your request.");
            }
        }
    }
}
