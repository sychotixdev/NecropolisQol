﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using NecropolisQol.Models;
using Newtonsoft.Json;

namespace NecropolisQoL;

public class NecropolisQolSettings : ISettings
{
    [JsonIgnore]
    public List<(string Id, string Name)> DevotionMods = new List<(string, string)>
    {
        ("NecropolisItemQuantity", "Item Quantity"),
        ("NecropolisItemRarity", "Item Rarity"),
        ("NecropolisExperienceGain", "Experience Gain"),
        ("NecropolisPackSpawnsShrineOnDeath", "Pack Spawns Shrine On Death"),
        ("NecropolisLastPackSummonsNamelessSeer", "Last Pack Summons Nameless Seer"),
        ("NecropolisTormentedSpiritOnPackDeath", "Tormented Spirit On Pack Death"),
        ("NecropolisStrongboxOnPackDeath", "Strongbox On Pack Death")
    };

    [JsonIgnore]
    public List<(string Id, string Name)> NormalMods = new List<(string, string)>
    {
        ("NecropolisMaximumLife", "Maximum Life"),
        ("NecropolisDamage", "Damage"),
        ("NecropolisSpeed", "Speed"),
        ("NecropolisMovementSpeed", "Movement Speed"),
        ("NecropolisExtraChaosDamage", "Extra Chaos Damage"),
        ("NecropolisExtraElementalDamage", "Extra Elemental Damage"),
        ("NecropolisExtraFireDamage", "Extra Fire Damage"),
        ("NecropolisExtraColdDamage", "Extra Cold Damage"),
        ("NecropolisExtraLightningDamage", "Extra Lightning Damage"),
        ("NecropolisResistance", "Resistance"),
        ("NecropolisOverwhelmPhysicalDamageReduction", "Overwhelm Physical Damage Reduction"),
        ("NecropolisPhysicalDamageReduction", "Physical Damage Reduction"),
        ("NecropolisIncreasedCriticalStrikes", "Increased Critical Strikes"),
        ("NecropolisFreeze", "Freeze"),
        ("NecropolisIgnite", "Ignite"),
        ("NecropolisShock", "Shock"),
        ("NecropolisChill", "Chill"),
        ("NecropolisPoison", "Poison"),
        ("NecropolisStun", "Stun"),
        ("NecropolisAreaOfEffect", "Area Of Effect"),
        ("NecropolisExtraProjectiles", "Extra Projectiles"),
        ("NecropolisSlowImmune", "Slow Immune"),
        ("NecropolisBlock", "Block"),
        ("NecropolisSuppression", "Suppression"),
        ("NecropolisEvasion", "Evasion"),
        ("NecropolisUnaffectedByCurses", "Unaffected By Curses"),
        ("NecropolisReflectAilments", "Reflect Ailments"),
        ("NecropolisEnergyShield", "Energy Shield"),
        ("NecropolisLifeRegeneration", "Life Regeneration"),
        ("NecropolisAccuracy", "Accuracy"),
        ("NecropolisFrenzyCharge", "Frenzy Charge"),
        ("NecropolisPowerCharge", "Power Charge"),
        ("NecropolisEnduranceCharge", "Endurance Charge"),
        ("NecropolisHinder", "Hinder"),
        ("NecropolisMaim", "Maim"),
        ("NecropolisRemoveKillerFlaskChargesWhenSlain", "Remove Killer Flask Charges When Slain"),
        ("NecropolisMonsterDamageFromAlivePack", "Monster Damage From Alive Pack"),
        ("NecropolisEnhancePackDamageOnDeath", "Enhance Pack Damage On Death"),
        ("NecropolisEnhanceStrongestPackMember", "Enhance Strongest Pack Member"),
        ("NecropolisEnhanceStrongestPackMemberWithAlivePack", "Enhance Strongest Pack Member With Alive Pack"),
        ("NecropolisEnhanceOtherTypedPacksOnDeath", "Enhance Other Typed Packs On Death"),
        ("NecropolisMeteorShowerOnPackDeath", "Meteor Shower On Pack Death"),
        ("NecropolisMonsterLevel", "Monster Level"),
        ("NecropolisMonsterLifeFromAlivePack", "Monster Life From Alive Pack"),
        ("NecropolisMonsterAttackAndCastSpeedFromAlivePack", "Monster Attack And Cast Speed From Alive Pack"),
        ("NecropolisEnhancePackLifeOnDeath", "Enhance Pack Life On Death"),
        ("NecropolisEnhancePackSpeedOnDeath", "Enhance Pack Speed On Death")
    };

    public static readonly string DEFAULT_MONSTER = "Default";

    [JsonIgnore]
    public List<(string Id, string Name)> AllMonsters = new List<(string, string)>
    {
        (DEFAULT_MONSTER, DEFAULT_MONSTER), // This is a dummy "Default" monster 

        ("Zombie", "Zombies"),
        ("Sandspitter3", "Crustacean Snipers"),
        ("Sandspitter2", "Gravel Eaters"),
        ("Sandspitter", "Sandspitters"),
        ("Crabs", "Savage Crabs"),
        ("WaterElemental", "Water Elementals"),
        ("WaterElemental3", "Crashing Waves"),
        ("WaterElemental4", "Bubbling Waves"),
        ("WaterElemental2", "Clearwater Striders"),
        ("Cannibal", "Cannibals"),
        ("Rhoa", "Rhoas"),
        ("RhoaLarge", "Large Rhoas"),
        ("ZombiesMired2", "Rotting Damned"),
        ("ZombiesMired", "Zombies"),
        ("necropolis-frogs", "Frogs"),
        ("Ratsss", "Rats"),
        ("necropolis-rats", "Rats"),
        ("Crustaceans2", "Invading Crustaceans"),
        ("Crustaceans3", "Shield Crabs"),
        ("Crustaceans", "Shield Crabs"),
        ("RhoaSkeleton", "Skeleton Rhoas "),
        ("MeleeSkeleton3", "Sandworn Watchmen"),
        ("MeleeSkeleton2", "Timeless Wanderers"),
        ("MeleeSkeleton4", "Brittle Bandits"),
        ("MeleeSkeleton5", "Ash Harvesters"),
        ("MeleeSkeletonwhitenec", "Brittle Born-Agains"),
        ("MeleeSkeleton6", "Vault Guardians"),
        ("MeleeSkeleton7", "Sandy Guardians"),
        ("MeleeSkeletonvoll", "Voll's Fallen"),
        ("MeleeSkeleton8", "Ancient Workers"),
        ("MeleeSkeleton", "Skeletons"),
        ("MeleeSkeletonNecro", "Skeletons"),
        ("RangedSkeleton3", "Weathered Watchmen"),
        ("RangedSkeleton2", "Timeless Hunters"),
        ("RangedSkeletonwhitenec", "Bow Skeletons "),
        ("RangedSkeleton4", "Brittle Lookouts"),
        ("RangedSkeleton5v2", "Brittle Poachers"),
        ("RangedSkeleton5", "Sandy Lookouts"),
        ("RangedSkeletonvoll", "Voll's Vanguards"),
        ("RangedSkeleton", "Bow Skeletons "),
        ("RangedSkeletonNecro", "Bow Skeletons"),
        ("CasterSkeleton", "Caster Skeletons "),
        ("SeaSpawn", "Sea Spawn"),
        ("SeaWitch2", "Singing Sirens"),
        ("SeaWitch", "Sea Witches"),
        ("Squid2", "Cavern Drifters"),
        ("SquidSewer", "Sewer Drifters"),
        ("Squid", "Drifting Eyes"),
        ("GoatmanMelee2", "Goatmen Stompers"),
        ("GoatmanMelee3v2", "Hill Devils"),
        ("GoatmanMelee3", "Alpine Devils"),
        ("GoatmanMelee", "Goatmen"),
        ("GoatmanMeleeLightning", "Bearded Devils"),
        ("GoatManCaster", "Caster Goatmen"),
        ("Kiweth", "Kiweth"),
        ("WheelSkeleton", "Dreadwheels"),
        ("NecromancersZombies", " Zombies and Necromancer"),
        ("PrisonStalker", "Ghouls"),
        ("TarZombie", "Fetid Shamblers"),
        ("PrisonSpider", "Cell Crawlers"),
        ("HellionCry", "Ruins Hellions"),
        ("Hellion", "Flame Hellions"),
        ("MeleeGhost2", "Spectral Corsairs"),
        ("MeleeGhost", "Ghosts"),
        ("RangedGhost", "Ranged Ghosts"),
        ("BrineVessel", "Brine Vassals"),
        ("AliraBanditCaster", "Alira's Archers"),
        ("AliraBanditMelee", "Alira's Bandits"),
        ("BanditMelee", "Callow Thugs"),
        ("BeastParasite", "Enraptured Beasts"),
        ("Beast2", "Thicket Hulks"),
        ("Beast", "Hairy Bonecrunchers"),
        ("BeastDegenAura", "Shaggy Monstrosities"),
        ("BeastSlam2", "Forest Beasts"),
        ("BeastSlam", "Thicket Hulks"),
        ("BeastVuln", "Armour Crunchers"),
        ("DropBear2", "Tunnelfiends"),
        ("DropBear3", "Woods Ursae"),
        ("DropBear", "Plummeting Ursae"),
        ("FireSnake", "Bramble Cobras"),
        ("GolemFire", "Flame Salvagers"),
        ("Insectburrow", "Carrion Burrowers"),
        ("Insect", "Carrion Swarmers"),
        ("KraitynBanditMelee", "Kraityn's Aspirants"),
        ("KraitynBanditRanged", "Kraityn's Scouts"),
        ("MeleeSnake2", "Bramble Cobras"),
        ("MeleeSnake", "Serpentine Constructs"),
        ("MinebatScreech", "Screechers"),
        ("MineBat", "Howlers"),
        ("MoldyZombie2", "Sutured Aberrations"),
        ("MoldyZombie", "Devout Dead"),
        ("MoldyZombieNecromancer", "Shambling Cadavers"),
        ("Monkey2", "Dread Apes"),
        ("Monkey", "Blood Apes"),
        ("MonkeyEnrage", "Carnage Apes"),
        ("MonkeyEnrageSilver", "Stygian Apes"),
        ("MonkeyRanged", "Barrow Apes"),
        ("MossMonster2", "Hulking titans"),
        ("MossMonster3", "Stalag Nightmares"),
        ("MossMonster4", "Ruined Titans"),
        ("MossMonster5", "Mountain Fragments"),
        ("MossMonster", "Stone Titans"),
        ("OakBanditMelee", "Oak's Devoted"),
        ("OakBanditRanged", "Oak's Snipers"),
        ("ParasiteCrab", "Enraptured Crabs"),
        ("PlagueInsect", "Carrion Wasps"),
        ("RootSpider", "Devourers"),
        ("SandLeaperparasite", "Dirt Scrabblers"),
        ("SandleaperWhip", "Lowlands Hoppers"),
        ("SandLeaper3", "Sand Skitterers"),
        ("SandLeaper", "Dirt Scrabblers"),
        ("Scarecrow", "Scarecrows"),
        ("SpiderSabre", "Lurking Venoms"),
        ("SpiderScaleViper3", "Mutant Arach"),
        ("SpiderScaleViper2", "Crypt Ambushers"),
        ("SpiderScaleViper4", "Lurking Venom"),
        ("SpiderScaleViperProj", "Venomous Spiders"),
        ("SpiderScaleViper", "Venomous Spiders"),
        ("SpiderScale", "Ink Spinners"),
        ("SpiderSmall2", "Spindle Spiders"),
        ("SpiderSmall3", "Cave Skitterers"),
        ("SpiderSmall", "Small Spiders"),
        ("SpiderThorn3", "Arakaali's Daughters"),
        ("SpiderThorn4", "Leaping Spiders"),
        ("SpiderThorn2", "Arakaali's Daughters"),
        ("SpiderThorn", "Vaal Recluses"),
        ("TwigMonster", "Skeleton Spriggans"),
        ("VaalCommander", "Undead Vaal Commanders"),
        ("VaalFragment2", "Ancient Sentries"),
        ("VaalFragment", "Ancient Constructs"),
        ("VaalGuardMelee", "Undead Vaal Guards"),
        ("VaalGuardMeleeDagger", "Undead Vaal Bladedancers"),
        ("VaalGuardRanged", "Undead Vaal Archers"),
        ("VaalSkeletonCasterCold", "Frost Archmages"),
        ("VaalSkeletonCasterFire", "Flame Archmages"),
        ("VaalSkeletonCasterLightning", "Thunder Archmages"),
        ("VaalSkeletonMeleeKnight", "Vaal Fallen"),
        ("VaalSkeletonMeleeFire", "Flame Guardians"),
        ("VaalSkeletonMeleeCold", "Frost Guardians"),
        ("VaalSkeletonMeleeLightning", "Thunder Guardians"),
        ("VaalSnakeRanged", "Spine Constructs"),
        ("VaalSnake", "Serpentine Constructs"),
        ("Wasp", "Plague Wasps"),
        ("AxisCaster", "Blackguard Mages"),
        ("AxisCasterArc", "Blackguard Arcmages"),
        ("AxisEliteSoldier", "Blackguard Elites"),
        ("AxisExperimenter", "Blackguard Experimenters"),
        ("AxisFootsoldierLunaris", "Lunar Scouts"),
        ("AxisFootsoldier", "Blackguard Soldiers"),
        ("BlackguardZombiePackLeader", "Pale Blackguards "),
        ("BlackguardZombie", "Pale Blackguards "),
        ("BloodElemental2", "Fevered Clots"),
        ("BloodElementalDoedre", "Doedre's Ichor"),
        ("BloodElemental", "Blood Elementals"),
        ("BurnedSkeletonMelee", "Burned Skeletons"),
        ("CityStalkerCasterPhysical", "Undying Evangelists"),
        ("CityStalkerLightning", "Undying Aristocrats "),
        ("CityStalkerLightningPack", "Undying Aristocrats "),
        ("CityStalkerCold3", "Undying Inductors"),
        ("CityStalkerCold2", "Undying Subjects"),
        ("CityStalkerCold4leader", "Undying Workers"),
        ("CityStalkerCold6", "Undying Operators"),
        ("CityStalkerCold5", "Undying Labourers"),
        ("CityStalkerCold4", "Undying Workers"),
        ("CityStalkerCold", "The Undying"),
        ("CityStalkerColdLibrary", "The Undying Archivists"),
        ("CityStalkerFire", "Undying Wretches"),
        ("DockSkeletonMelee", "Brittle Skeletons"),
        ("DockSkeletonRanged", "Ranged Brittle Skeletons"),
        ("DockWorkerMeleeleader", "Dockhand Wraiths"),
        ("DockWorkerMelee", "Dockhand Wraiths"),
        ("DockWorkerRangerleader", "Ranged Dockhand Wraiths"),
        ("DockWorkerRanger", "Ranged Dockhand Wraiths"),
        ("FemaleDemonMeleeLunaris", "Night Horrors"),
        ("FemaleDemonMelee", "Whipping Miscreations"),
        ("FemaleDemonRangedLunaris", "Lunaris Concubines"),
        ("FemaleDemonRanged", "Tentacle Miscreations"),
        ("FireRockElemental", "Cinder Elementals"),
        ("FlameBearerBlue", "Voidbearers"),
        ("FlameBearerYellow", "Voidbearers"),
        ("Frog2", "Filth Maws"),
        ("Frog", "Fetid Maws"),
        ("GemFrog2", "Vaulting Croakers"),
        ("GemFrog", "Chimeric Croakers"),
        ("Grappler2", "Shavronne's Gemlings"),
        ("Grappler3", "Undying Grapplers"),
        ("Grappler", "Undying Grapplers"),
        ("GreenSquid", "Soulless Watchers"),
        ("HairySpider2", "Buried Tarantulas"),
        ("HairySpider", "Brooding Tarantulas"),
        ("Iguana2", "Feral Chimerals"),
        ("Iguana", "Plumed Chimerals"),
        ("InsectSpawner", "Carrion Swarmers"),
        ("LargeSkeletonMelee", "Large Skeletons"),
        ("MeleeSkeletonSword", "Sword Skeletons"),
        ("ModularDemon", "Piety's Miscreations"),
        ("ModularDemonCold", "Drenched Miscreations"),
        ("ModularDemonFire", "Burned Miscreations"),
        ("ModularDemonLightning2", "Augmented Dead"),
        ("ModularDemonLightning", "Shocked Miscreations"),
        ("ModularDemonColdAxis2", "Drenched Miscreations"),
        ("ModularDemonFireAxis2", "Burned Miscreations"),
        ("ModularDemonLightningAxis2", "Shocked Miscreations"),
        ("PyromaniacFire", "Undying Incinerators"),
        ("PyromaniacPoison", "Undying Alchemists"),
        ("RangedSkeletonLightning", "Lightning Ranged Skeletons"),
        ("Ribbon", "Assault Ribbons"),
        ("SkeletonMeleeChilled", "Cold Skeletons"),
        ("infestedserpent", "Infested Serpents"),
        ("SnakeScorpion2", "Sand Serpents"),
        ("SnakeScorpion", "Spine Serpents"),
        ("Spiker", "Porcupine Goliaths"),
        ("SpittingSnake", "Glade Mambas"),
        ("UndyingOutcastRavager", "Undying Outcasts"),
        ("UndyingOutcast", "Undying Outcasts"),
        ("WaterElementalPure", "Pure Water"),
        ("BellySkeletonMelee2", "Arranged Anatomy"),
        ("BellySkeletonMelee", "Gut Flayers"),
        ("BellySkeletonRanged", "Gut Spikers"),
        ("BlackguardJaegerMelee", "Jaegers"),
        ("BlackguardJaegerRanged", "Jaeger Archers"),
        ("BlackguardJaegerZombie", "Risen Jaegers"),
        ("BlackguardZombie2", "Pale Blackguards"),
        ("CockerelMelee2", "Feral Fowls"),
        ("CockerelMelee", "Cockerels"),
        ("CockerelRanged", "Cockerel Archers"),
        ("Eye2", "Wide-eyed Assemblages"),
        ("Eye", "Wandering Eyes"),
        ("FireHoundstandalone", "Fire Hounds"),
        ("FireHound", "Fire Hounds"),
        ("Gladiator", "Mad Gladiators"),
        ("HalfSkeletonMelee2", "Bloodstained Warriors"),
        ("HalfSkeletonMelee", "Bonewarped Warriors"),
        ("HalfSkeletonRanged2", "Bloodstained Archers"),
        ("HalfSkeletonRanged", "Bonewarped Archers"),
        ("Handfinger", "Crawlers"),
        ("Heitiki", "Inua Totems"),
        ("HellionIce", "Mountain Hellions"),
        ("KaomMelee", "Kaom's Warriors"),
        ("KaomRanged", "Kaom's Archers"),
        ("Melee", "[DNT] Melee"),
        ("MineBat2", "[DNT] MineBat2"),
        ("Miner2", "Pocked Masons and Flamebringers"),
        ("Miner", "Pocked Miners and Lanternbearers"),
        ("ParasiteVulture2", "Scavenging Vultures"),
        ("ParasiteVulture3", "Rotting Vultures"),
        ("ParasiteVulture", "Infested Vultures"),
        ("Pitbull2", "Vicious Hounds"),
        ("Pitbull", "Pitbulls"),
        ("PuiTotem", "Volcanic Totems"),
        ("PusElemental", "Pus Elementals"),
        ("SnakeFlame", "Night Adders"),
        ("Taster", "Noisome Ophidians"),
        ("Vendigo", "Vendigos"),
        ("VollSkeletonMelee", "Voll's Fallen"),
        ("VollSkeletonRanged", "Voll's Vanguards"),
        ("WaterSpiralSewer", "Muck Vortexes"),
        ("WaterSpiral2", "Vortexes"),
        ("WaterSpiral", "Maelströms"),
        ("AnimatedBust", "Living Memorials"),
        ("Banshee2", "Shrieking Witch"),
        ("Banshee", "Wailing Widows"),
        ("BlackguardInquisitor", "Temple Justicars"),
        ("BlackguardInquisitorExtra", "Temple Justicars"),
        ("BlackguardTorturer", "Truth Extractors"),
        ("BlackSkeletonCaster", "Desecrated Saints"),
        ("BlackSkeletonMelee", "Brittle Clerics"),
        ("GoldSkeletonCaster", "Gleaming Saints"),
        ("HolyFireElemental2", "Solar Elementals"),
        ("HolyFireElemental", "Prayer Guards"),
        ("IndoctrinatedConvert", "Indoctrinated Slaves"),
        ("KitavaBlackguardCleric", "Crimson Scholars"),
        ("KitavaBlackguardInquisitor2", "Twisted and Depraved Justicars"),
        ("KitavaBlackguardInquisitor", "Twisted Justicars"),
        ("KitavaBlessedSister", "Sanguine Sisters"),
        ("KitavaCultistMeleeHerald", "Kitava Zealots"),
        ("KitavaCultistMelee", "Kitava Zealots"),
        ("KitavaDemon", "Kitava's Heralds"),
        ("KitavaReligionTemplar", "Heretical Wards"),
        ("KitavaSlaveCatcherJudge", "Depraved and Deranged Slavers"),
        ("KitavaSlaveCatcher", "Depraved and Deranged Slavers"),
        ("KitavaTemplarJudge", "Heretical Adjudicators"),
        ("MotherofFlames", "Flame Mothers"),
        ("RisenSaints", "Risen Saints"),
        ("OriathBlackguard", "Oriath Enforcers"),
        ("OriathGhostMelee", "Vengeful Souls"),
        ("OriathGhostRanged", "Desperate Souls"),
        ("Pauper", "Restless Paupers"),
        ("PurgeHound", "Purge Hounds"),
        ("ReligionTemplar", "Temple Guardians"),
        ("SecretPolice", "Blackguard Stalkers"),
        ("SkeletonMelee", "[DNT] SkeletonMelee"),
        ("SlaveCatcher", "Slave Catchers"),
        ("SlaveDriver", "Slave Drivers"),
        ("TemplarJudge", "Temple Adjudicators"),
        ("TemplarJudgeExtra", "Temple Adjudicators"),
        ("BanditAnimalEquipment2", "Bandit Acolytes"),
        ("BanditAnimalEquipment3", "Ralakesh Bandits"),
        ("BanditAnimalEquipment5", "Heartless Deserters"),
        ("BanditAnimalEquipment4", "Treacherous Deserters"),
        ("BanditAnimalEquipment", "Bandit Acolytes"),
        ("CageSpider2", "Sandworn Slaves"),
        ("CageSpider", "Rattling Condemned"),
        ("CrabParasite2", "Infested Crabs"),
        ("CrabParasite", "Savage Crabs"),
        ("FrogParasite", "Infested Maws"),
        ("FrogParasite2", "Infested Maws"),
        ("HairySpiderParasite", "Enraptured Arachnids"),
        ("HellionParasite", "Enslaved Hellions"),
        ("KaomArcher", "Tukohama Scouts"),
        ("KaomWarrior", "Tukohama Warriors"),
        ("SandspitterCold", "Crustecean Snipers"),
        ("MonkeyChiefParasite", "Infested Apes"),
        ("MonkeyParasite", "Infested Apes"),
        ("OriathanCitizenZombieKitava", "Experimented Dead"),
        ("OriathanCitizenZombie", "Noble Dead"),
        ("Parasite2", "Ravenous Parasites"),
        ("Parasite", "Parasites"),
        ("RhoaParasite", "Infested Rhoas"),
        ("RockGolemCoral", "Animated Reefs"),
        ("SnakeSpitParasite", "Host Cobras"),
        ("SquidParasite2", "Swarthy Molluscs"),
        ("SquidParasite", "Brine Vassals"),
        ("DropBearParasite2", "Infested Tunnelfiends"),
        ("DropBearParasite", "Infected Ursae"),
        ("GhostCaster", "Spectral Mages"),
        ("PoisonGolem", "Volatile Poison "),
        ("RubbleGolem", "Rubble Golems"),
        ("SkeletonCasterFireVaal", "Vaal Arsonists"),
        ("SkeletonKnightLargeVaal", "Vaal Gargantuans"),
        ("SkeletonKnightVaalCold", "Vaal Frost Guardians"),
        ("SkeletonKnightVaal", "Vaal Fallen"),
        ("SkeletonMeleeColdVaal", "[DNT] SkeletonMeleeColdVaal"),
        ("SkeletonRangedVaal", "Vaal Slayers"),
        ("SnakeFlameParasite", "Host Adders"),
        ("SpiderPlague2", "Diseased Arachnids"),
        ("Spidermali", "Maligaro's Inspiration"),
        ("SpiderPlague", "Plagued Arachnids"),
        ("SpiderPlated", "Hybrid Arachnids"),
        ("SpiderPlatedFire", "Scalding Arachnids"),
        ("StatueMeleeFemale", "Idolised Saints"),
        ("StatueMeleeMale", "Idolised Saints"),
        ("StatueRangedFemale", "Ranged Idolised Saints"),
        ("Wraith", "Wraiths"),
        ("ChromeGargoyle", "Chromatic Gargoyles"),
        ("CityStalkerArmouredBlueNoSnap", "Undying Initiates"),
        ("CityStalkerArmouredBlueSnap", "Undying Patrician"),
        ("CityStalkerArmouredBlueSwa", "Undying Degenerates"),
        ("CityStalkerArmouredBlue", "Stinking Undying"),
        ("CityStalkerTar", "Undying Dregs"),
        ("DemonLunaris", "[DNT] DemonLunaris"),
        ("DockworkerChampion", "[DNT] DockworkerChampion"),
        ("FarmerSkeleton", "Brittle Farmers"),
        ("GemFrogChrome", "Chrome-Infused Croakers"),
        ("GhostHeroCaster", "[DNT] GhostHeroCaster"),
        ("GhostHeroMelee", "Spectral Soldiers"),
        ("IguanaChrome", "Chrome-Infused Chimerals"),
        ("LunarisCannibalFemale", "Lunaris Zealots"),
        ("LunarisCannibalMale", "[DNT] LunarisCannibalMale"),
        ("LunarisDemonRanged", "[DNT] LunarisDemonRanged"),
        ("LunarisEliteSoldierMeleeleader", "Lunaris Elites"),
        ("LunarisEliteSoldierMelee", "Lunaris Elites"),
        ("LunarisSoldierCaster", "Lunarsworn Wintermages"),
        ("LunarisSoldierMelee", "[DNT] LunarisSoldierMelee"),
        ("ModularDemonLunaris", "Lunar and Moontouched Miscreations"),
        ("PoisonFrog", "[DNT] PoisonFrog"),
        ("PoisonInsect", "Scum Crawlers"),
        ("PusElementalPoison", "Mephitic Maidens"),
        ("RibbonBlue", "Assault Ribbons"),
        ("RibbonLargeCold", "[DNT] RibbonLargeCold"),
        ("RibbonLargeColdVersion2", "[DNT] RibbonLargeColdVersion2"),
        ("RibbonLargeFire", "[DNT] RibbonLargeFire"),
        ("RibbonRed", "Assault Ribbons"),
        ("SewerGoliath", "Sewer Goliaths"),
        ("SkeletonMeleeRoman", "Brittle Gladiators"),
        ("SnakeSpitterPoison", "Acid Slitherers"),
        ("SolarisEliteSoldierMelee", "Solaris Soldiers"),
        ("SolarisFireElemental", "[DNT] SolarisFireElemental"),
        ("SolarisLion", "Auric Lions"),
        ("SolarisSoldierCaster", "Solarsworn Infernomages"),
        ("SpikerChrome", "Chrome-Infused Goliaths"),
        ("WaterElementalPure2", "Holy Water"),
        ("Bird2", "Adolescent Rhexes"),
        ("Bird", "Wild Rhexes"),
        ("BirdArmoured", "Escaped Rhexes"),
        ("BlackScorpion", "Black Scorpions"),
        ("BleachedSkeleton", "Sun Bleached Fallen "),
        ("BleachedSkeletonRanged", "Sun Bleached Vanguards"),
        ("ChaosElemental", "[DNT] ChaosElemental"),
        ("ClockworkGolem", "Refinery Constructs"),
        ("CrabBleached", "Bleached Crawlers"),
        ("CrustaceanBleached", "Bleached Crustaceans"),
        ("DoedreWraith", "Doedre's Suffering"),
        ("FlameBearerBlight", "Maligaro's Cruelty and Passion"),
        ("GemFrogVivid", "[DNT] GemFrogVivid"),
        ("GoatmanChampion", "[DNT] GoatmanChampion"),
        ("HalfSkeletonMeleePart2", "Gut Flayers"),
        ("HalfSkeletonRangedPart2", "Gut Spikers"),
        ("IceElemental", "Brittle Shards"),
        ("IguanaVivid", "[DNT] IguanaVivid"),
        ("Lynx", "Mountain Lynx"),
        ("MarakethRider", "Rhexback Warriors"),
        ("MossMonsterCold", "Frozen Titans"),
        ("NessaCrabBleached", "[DNT] NessaCrabBleached"),
        ("SandLeaper2", "[DNT] SandLeaper2"),
        ("ShavronneCityStalker2", "Shavronne's Envy"),
        ("ShavronneCityStalker", "Shavronne's Legacy "),
        ("ShavronneClockworkGolem", "Shavronne's Vision "),
        ("SkeletonMeleeBloody", "[DNT] SkeletonMeleeBloody"),
        ("SkeletonMeleeBloodyChampion", "Bloodsoaked Fallen"),
        ("SkeletonMeleeCrystal", "Crystallised Fallen"),
        ("SkeletonRangedBloody", "[DNT] SkeletonRangedBloody"),
        ("SkeletonRangedBloodyChampion", "Bloodsoaked Vanguards"),
        ("SkeletonRangedCrystal", "Crystallised Sentries"),
        ("SmallCrabBleached", "Sulphurspawn"),
        ("TasterPart2", "Noisome Ophidians"),
        ("WaterElementalSulphur", "Sulphuric Striders"),
        ("WaterSpiralBlood", "Blood Vortexes"),
        ("WhiteScorpion", "Sulphuric Scoprions"),
        ("Wolf", "Arctic Wolves"),
        ("WolfCold", "Snow Wolves"),
        ("YellowScorpion", "Sand Scorpions "),
        ("ZombieMummy", "Dessicated Maraketh"),
        ("BlackguardKitavaJudge", "Depraved and Enthralled Enforcers"),
        ("BlackguardKitava", "[DNT] BlackguardKitava"),
        ("IceElementalGlass", "Window Shards"),
        ("IndoctrinatedConvert2", "Deformed Slaves"),
        ("KitavaSkeleton", "Desecrated Thralls"),
        ("Mauler", "Handmaidens"),
        ("MotherOfFlamesZombie", "[DNT] MotherOfFlamesZombie"),
        ("PurgeHoundKitava", "Purge Hounds"),
        ("Regurgitator", "Warped Regurgitators"),
        ("SlaveDriverKitava", "Crazed Drivers"),
        ("SpearCandleCultistKitava", "Feast Attendants"),
        ("Templar2HKitava", "[DNT] Templar2HKitava"),
        ("TorturerKitava", "Defiled and Corrupted Extractors"),
        ("WickerMan", "Wicker Men"),
        ("PitbullCockerel", "[DNT] Pitbull and Cockerel"),
        ("HalfSkeletonPack", "[DNT] Half Skeletons"),
        ("ArmouredBullExtra", "Fighting Bulls"),
        ("BeastParasiteExtra", "[DNT] BeastParasiteExtra"),
        ("BlackguardStalkerExtra", "[DNT] BlackguardStalkerExtra"),
        ("BlessedSisterExtra", "[DNT] BlessedSisterExtra"),
        ("BullExtra2", "Avalanche Riders"),
        ("BullExtra", "Grazing Taurus"),
        ("ColdGoatmanCasterExtra", "Alpine Shamans"),
        ("ColdSkeletonCasterExtra3", "Frost Preachers"),
        ("ColdSkeletonCasterExtra2", "Frost Bishops"),
        ("ColdSkeletonCasterExtra", "Caster Cold Skeletons"),
        ("ColdSkeletonCasterNecro", "Caster Cold Skeletons"),
        ("ColdSkeletonMeleeExtra", "Cold Skeletons"),
        ("ColdSkeletonMeleeNecro", "Cold Skeletons"),
        ("FireGoatmanCasterChampionExtra", "Caster Fire Champion Goatmen"),
        ("FireGoatmanCasterExtra2", "Goatmen Fire-raisers"),
        ("FireGoatmanCasterExtra", "Caster Fire Goatmen"),
        ("FireSkeletonCasterExtraNecro", "Fire Mage Skeletons"),
        ("FireSkeletonCasterExtra", "Fire Mage Skeletons"),
        ("FireSkeletonMeleeExtraNecro", "Fire Skeletons"),
        ("FireSkeletonMeleeExtra", "Fire Skeletons"),
        ("FireSkeletonRangedExtra2", "Burning Bowmen"),
        ("FireSkeletonRangedExtra3", "Disbanded Dead"),
        ("FireSkeletonRangedExtra", "Ranged Fire Skeletons"),
        ("GoldStatuesExtra", "[DNT] GoldStatuesExtra"),
        ("LightningGoatmanCasterExtra", "Bearded Shamans"),
        ("LightningSkeletonCasterExtra", "Caster Lightning Skeletons"),
        ("LightningSkeletonCasterNecro", "Caster Lightning Skeletons"),
        ("LightningSkeletonMeleeExtraNecro", "Thunder Guardians"),
        ("LightningSkeletonMeleeExtra", "Lightning Skeletons"),
        ("LightningSkeletonRangedExtra2Necro", "Thunder Thralls"),
        ("LightningSkeletonRangedExtra2", "Brittle Poachers"),
        ("LightningSkeletonRangedExtra", "Ranged Lightning Skeletons"),
        ("LightningSkeletonRangedNecro2", "Sin Archers"),
        ("LightningSkeletonRangedNecro", "Ranged Lightning Skeletons"),
        ("RedRibbonLargeLightningExtra", "[DNT] RedRibbonLargeLightningExtra"),
        ("ReliquarianExtra", "[DNT] ReliquarianExtra"),
        ("SkeletonBeastExtra", "[DNT] SkeletonBeastExtra"),
        ("StygianRevenantExtra", "[DNT] StygianRevenantExtra"),
        ("AllFlameBreachFire", "Xoph's Demons"),
        ("AllFlameBreachCold", "Tul's Demons"),
        ("AllFlameBreachLightning", "Esh's Demons"),
        ("AllFlameBreachPhysical", "Uul-Netol's Demons"),
        ("AllFlameBreachChaos", "Chayula's Demons"),
        ("AllFlameSulphite", "Sulphite Golems"),
        ("AllFlameValako", "Valako Warriors"),
        ("AllFlameTukohama", "Tukohama Warriors"),
        ("AllFlameTawhoa", "Tawhoa Warriors"),
        ("AllFlameTasalio", "Tasalio Warriors"),
        ("AllFlameRongokurai", "Rongokurai Warriors"),
        ("AllFlameRamako", "Ramako Warriors"),
        ("AllFlameNgamahu", "Ngamahu Warriors"),
        ("AllFlameKitava", "Kitava Warriors"),
        ("AllFlameHinekora", "Hinekora Warriors"),
        ("AllFlameArohongui", "Arohongui Warriors"),
        ("AllFlameAbyss", "Abyss Monsters"),
        ("AllFlameScourgePale", "Demons of Beidat"),
        ("AllFlameScourgeFlesh", "Demons of Ghorr "),
        ("AllFlameScourgeDemon", "Demons of K'Tash"),
        ("AllFlameGemlingFire", "Fiery Attendants"),
        ("AllFlameGemlingFireberserk", "Fiery Attendants"),
        ("AllFlameGemlingCold", "Frigid Attendants"),
        ("AllFlameGemlingColdPhysBow", "Frigid Attendants"),
        ("AllFlameGemlingSummoner", "Voltaic Attendants"),
        ("AllFlameExpeditionGwennen", "Broken Circle Monsters"),
        ("AllFlameExpeditionRog", "Order of the Chalice Monsters"),
        ("AllFlameExpeditionTujen", "Black Scythe Monsters"),
        ("AllFlameExpeditionDannig", "Knights of the Sun Monsters"),
        ("AllFlameMeatsack", "Meatsacks"),
        ("AllFlameUntainedBeasts", "Primal Beasts"),
        ("AllFlameUntaintedMonkeys", "Primitive Apes"),
        ("AllFlameUntaintedRhoas", "Primoridal Rhoas "),
        ("AllFlameUntaintedSandspitters", "Fossil Eaters"),
        ("AllFlameUntaintedFrogs", "Bestial Maws"),
        ("AllFlameUntaintedIguanas", "Wild Chimerals"),
        ("AllFlameUntaintedRootspiders", "Ancient Devourers"),
        ("AllFlameAzmeriVoodoo", "Wildwood Cultists"),
        ("AllFlameAzmeriTreant", "Wildwood Treants"),
        ("AllFlameAzmeriGruthkul", "Wildwood Nameless Beasts"),
        ("AllFlameLegionKarui", "Timeless Karui Warriors"),
        ("AllFlameLegionEternal", "Timeless Eternal Warriors"),
        ("AllFlameLegionMaraketh", "Timeless Maraketh Warriors"),
        ("AllFlameLegionTemplar", "Timeless Templar Warriors"),
        ("AllFlameLegionVaal", "Timeless Vaal Warriors"),
        ("chaos-ghouls", "Ghouls"),
        ("chaos-cellcrawlers", "Cell Crawlers"),
        ("chaos-bandits", "Oak Bandits"),
        ("chaos-bandits2", "Kraityn Sentries"),
        ("chaos-bandits3", "Callow Thugs"),
        ("chaos-bandits4", "Alira Bandits"),
        ("chaos-bandits5", "Callow Snipers"),
        ("chaos-bandits6", "Acolyte Bandits"),
        ("chaos-bandits7", "Bandit Acolytes"),
        ("chaos-bandits8", "Acolytes"),
        ("chaos-beasts1", "Cave Beasts"),
        ("beasts2", "Bone Crunchers"),
        ("chaos-beasts2", "Bone Crunchers"),
        ("chaos-beasts3", "Hairy Bonecrunchers"),
        ("chaos-beasts4", "Shaggy Monstrosities"),
        ("chaos-beasts5", "Thicket Hulks"),
        ("chaos-beasts-parasite6", "Enraptured Beasts"),
        ("chaos-animals", "Armour Crunchers"),
        ("chaos-beasts6", "Skeletal Beasts"),
        ("chaos-fragments-emanant1", "Ancient Constructs"),
        ("chaos-ghosts1", "Cursed Mariners"),
        ("chaos-ghosts2", "Spectral Corsairs"),
        ("chaos-ghosts3", "Phantom Occultists"),
        ("chaos-ghosts4", "Spectral Mages"),
        ("chaos-animals2", "Goatmen"),
        ("chaos-goatmen2", "Bearded Devils"),
        ("chaos-goatmen3", "Goatmen Shamans"),
        ("chaos-goatmen4", "Leaping Goatmen"),
        ("chaos-goatmen5", "Leaping Bearded Devils"),
        ("chaos-goatmen6", "Alpine Devils"),
        ("chaos-goatmen7", "Hill Devils"),
        ("chaos-lion1", "Dune Hellions"),
        ("chaos-lion2", "Flame Hellions"),
        ("chaos-icehellions1", "Mountain Hellions"),
        ("chaos-hellions-parasite1", "Enslaved Hellions"),
        ("chaos-animals4", "Flame Hellions"),
        ("chaos-monkeys1", "Dread Primates"),
        ("chaos-monkeys2", "Stygian Apes"),
        ("chaos-monkeys3", "Carnage Apes"),
        ("chaos-monkeys4", "Blood Apes"),
        ("chaos-monkeys5", "Dread Primates"),
        ("chaos-monkeys-parasite6", "Infested Apes"),
        ("chaos-mossmonsters1", "Stalag Nightmares"),
        ("chaos-mossmonsters2", "Ruined Titans"),
        ("chaos-mossmonsters3", "Rumbling Masses"),
        ("chaos-mossmonsters4", "Hulking Titans"),
        ("chaos-necros1", "Flame Guardians"),
        ("chaos-necros2", "Sparking Mages"),
        ("chaos-necros3", "Brittle Thiefs"),
        ("chaos-necros4", "Bone Wardens"),
        ("chaos-necros5", "Fetid Shamblers"),
        ("chaos-necros6", "Bone Rhoas"),
        ("chaos-necros7", "Frost Guardians"),
        ("chaos-animals5", "Rhoas"),
        ("chaos-rhoas2", "Rhoa Mares"),
        ("chaos-rhoas3", "Bone Rhoas"),
        ("chaos-rhoas4", "Murk Runners"),
        ("chaos-rhoas-skeletons5", "Bone Rhoas"),
        ("chaos-rhoas-parasite6", "Infested Rhoas"),
        ("chaos-sandspitters-emanant1", "Granite Eaters"),
        ("chaos-sandspitters-emanant2", "Obsidian Eaters"),
        ("chaos-sandspitters-emanant3", "Crustecean Snipers"),
        ("chaos-cannibals1", "Fire Cannibals"),
        ("chaos-cannibals2", "Cannibals"),
        ("chaos-humanoids1", "Arsonist Cannibals"),
        ("chaos-seawitches-slithering-emanant1", "Singing Sirens"),
        ("chaos-seawitches-slithering2", "Merveil's Favoured"),
        ("chaos-seawitches-slithering3", "Blessed Spawn"),
        ("chaos-seawitches-slithering4", "Attendant Spawn"),
        ("chaos-seawitches-slithering5", "Merveil's Chosen"),
        ("chaos-seawitches-slithering6", "Merveil's Retainers"),
        ("chaos-shieldcrabs1", "Scrabbling Menaces"),
        ("chaos-shieldcrabs2", "Cave Crustaceans"),
        ("chaos-shieldcrabs3", "Deep Crustaceans"),
        ("chaos-shieldcrabs4", "Invading Crustaceans"),
        ("chaos-shieldcrabs-parasite5", "Infested Crustaceans"),
        ("chaos-shieldcrabs-bleached6", "Bleached Crawlers"),
        ("chaos-skeletons1", "Ancient Bonestalkers"),
        ("chaos-skeletons2", "Flame Guardians"),
        ("chaos-skeletons3", "Vault Hunters"),
        ("chaos-skeletons4", "Bone Wardens"),
        ("chaos-skeletons5", "Brittle Clerics"),
        ("chaos-skeletons6", "Frost Guardians"),
        ("splitskeles", "Brittle Archers"),
        ("chaos-skeletons-emanant7", "Brittle Archers"),
        ("chaos-skeletons8", "Ancient Bonestalkers"),
        ("chaos-skeletons9", "Frost Harbingers"),
        ("chaos-skeletons10", "Brittle Arsonists"),
        ("chaos-skeletons11", "Axiom Thunderguards"),
        ("chaos-skeletons12", "Scarecrows"),
        ("chaos-skeletons13", "Risen Saints"),
        ("chaos-skeletons14", "Risen Saints"),
        ("chaos-skeletons15", "Dreadwheels"),
        ("chaos-skeletons16", "Angered Remains"),
        ("chaos-skeletons16_2", "Timeless Wanderers"),
        ("chaos-skeletons17", "Brittle Born-agains"),
        ("chaos-skeletons18", "Sun Bleached Vanguards"),
        ("chaos-skeletons19", "Crystallised Fallen"),
        ("chaos-skeletons20", "Bloodstained Fallen"),
        ("chaos-skeletons21", "Bloodstained Vanguards"),
        ("chaos-skeletons22", "Desecrated Thralls"),
        ("chaos-snakes1", "Bramble Cobras"),
        ("chaos-snakes2", "Crypt Stalkers"),
        ("chaos-snakes3", "Night Adders"),
        ("chaos-snakes4", "Acid Slitherers"),
        ("chaos-snakes-parasite5", "Host Cobras"),
        ("chaos-animals7", "Night Adders"),
        ("chaos-spawns-slithering1", "Slimy Nemesises"),
        ("chaos-spiders1", "Ink Spinners"),
        ("chaos-spiders2", "Corrupted Arachs"),
        ("chaos-spiders3", "Cave Skitterers"),
        ("chaos-spiders4", "Spindle Spiders"),
        ("chaos-spiders6", "Leaping Spiders"),
        ("chaos-spiders7", "Noxious Tarantulas"),
        ("chaos-spiders-parasite8", "Enraptured Arachnids"),
        ("chaos-spiders9", "Plagued Arachnids"),
        ("chaos-spiders10", "Arakaali's Daughters"),
        ("chaos-vaal1", "Serpentine Constructs"),
        ("chaos-vaal2", "Vaal Constructs"),
        ("chaos-vaal3", "Vaal Fallen"),
        ("chaos-vaal4", "Vaal Fallen"),
        ("chaos-vaal5", "Vaal Frost Guardians"),
        ("chaos-vaal6", "Vaal Flame Guardians"),
        ("chaos-vaal7", "Vaal Bone Guardians"),
        ("chaos-vaal-emanant8", "Vaal Arsonists"),
        ("chaos-vaal-emanant9", "Vaal Frostguards"),
        ("chaos-vaal-emanant10", "Vaal Thunderguards"),
        ("chaos-vaal11", "Vaal Slayers"),
        ("chaos-waterelementals2", "Seething Brine"),
        ("chaos-waterelementals3", "Tide Striders"),
        ("chaos-waterelementals4", "Seething Brine"),
        ("chaos-waterelementals5", "Boiling Striders"),
        ("chaos-waterelementals6", "Clearwater Striders"),
        ("chaos-waterelementals7", "Sulphiric Striders"),
        ("chaos-zombies1", "Drowned"),
        ("chaos-zombies2", "Dripping Dead"),
        ("chaos-zombies3", "Shambling Cadavers"),
        ("chaos-zombies5", "Sutured Aberattions"),
        ("chaos-zombies6", "Rotting Damned"),
        ("chaos-zombies7", "Withered Husks"),
        ("chaos-zombies8", "Fetid Shamblers"),
        ("chaos-zombies9", "Drowned"),
        ("chaos-zombies10", "Risen Jaegers"),
        ("chaos-zombies11", "Noble Dead"),
        ("chaos-zombies12", "Noble Dead"),
        ("chaos-bandits-emanant1", "Archer Bandits"),
        ("chaos-emanant2", "Ancient Archers"),
        ("chaos-emanant3", "Glade Mambas"),
        ("chaos-emanant4", "Burning Skeletons"),
        ("chaos-emanant5", "Lightning Poachers"),
        ("chaos-emanant6", "Vault Hunters"),
        ("chaos-emanant7", "Undying Alchmists"),
        ("chaos-axis", "Blackguard Elites and Mages"),
        ("chaos-axis2", "Blackguard Soldiers and Mages"),
        ("chaos-axis3", "Blackguard Soldiers and Elites"),
        ("chaos-axis4", "Blackguard Soldiers"),
        ("chaos-experimenter2", "Piety Miscreations"),
        ("chaos-demons1", "Burned Miscreations"),
        ("chaos-demons2", "Drenched Miscreations"),
        ("chaos-demons3", "Shocked Miscreations"),
        ("chaos-demons4", "Whipping Miscreations"),
        ("chaos-demons5", "Voidbearers"),
        ("chaos-frog", "Fetid Maws"),
        ("chaos-frog2", "Filth Maws"),
        ("chaos-frog-parasite3", "Infested Maws"),
        ("chaos-froginsect", "Scum Crawlers"),
        ("chaos-guardians1", "Assault Ribbons"),
        ("chaos-guardians2", "Assault Ribbons"),
        ("chaos-guardians3", "Assault Ribbons"),
        ("chaos-guardians4", "Assault Ribbons"),
        ("chaos-guardians5", "Assault Ribbons"),
        ("chaos-insects", "Carrion Burrowers"),
        ("chaos-insects2", "Carrion Swarmers"),
        ("chaos-flamebearers", "Dockhand Wraiths"),
        ("chaos-flamebearers2", "Brittle Longshoremen"),
        ("chaos-firemonsters", "Cinder Elementals"),
        ("chaos-firemonsters2", "Cinder Elementals"),
        ("chaos-pyromaniacfire", "Undying Wretches"),
        ("chaos-pyromaniacfire2", "Undying Impalers"),
        ("chaos-pyromaniacpoison", "Sewage Crawlers"),
        ("chaos-necros8", "Burning Undead"),
        ("chaos-necros9", "Disbanded Dead"),
        ("chaos-necros10", "Brittle Bandits"),
        ("chaos-animals8", "Spine Serpents"),
        ("chaos-snakesscorpion2", "Serpent and Skitterers"),
        ("chaos-bloodelementals", "Blood Elementals"),
        ("chaos-bloodelementals2", "Blood Elementals"),
        ("chaos-bloodelementals3", "Fevered Clots"),
        ("chaos-citystalkers1", "Undying Workers"),
        ("chaos-citystalkers2", "Undying Untouchables"),
        ("chaos-citystalkers3", "Undying Archivists"),
        ("chaos-citystalkers-emanant4", "Undying Cultists"),
        ("chaos-citystalkers8", "Undying Workers"),
        ("chaos-humanoids2", "Stinking Undying"),
        ("chaos-undying", "Undying Outcasts"),
        ("chaos-dockworkers", "Dockhand Wraiths"),
        ("chaos-humanoids3", "Dockhand Wraiths"),
        ("chaos-humanoids4", "Alira Blood Guard"),
        ("chaos-humanoids5", "Oak Brutes"),
        ("chaos-humanoids6", "Blackguard Soldiers"),
        ("chaos-skeletonsdock", "Brittle Skeletons"),
        ("chaos-spikers", "Porcupine Golaiths"),
        ("chaos-kiweth", "Avian Retches"),
        ("chaos-kiweth2", "Gluttonous Gulls"),
        ("chaos-animals9", "Devourers"),
        ("chaos-skeletonlarge", "Ancient Bonestalkers"),
        ("chaos-citystalkers5", "Undying Aristocrats"),
        ("chaos-citystalkers6", "Undying Aristocrats"),
        ("chaos-citystalkers7", "Undying Socialites"),
        ("chaos-undyingiguana", "Plumed Chimerals"),
        ("chaos-iguana", "Plumed Chimerals"),
        ("chaos-gemfrog", "Chimeric Croakers"),
        ("chaos-gemfrogundying", "Chimeric Croakers"),
        ("chaos-animals10", "Soulless Watchers"),
        ("chaos-squidinsects", "Scum Crawlers"),
        ("chaos-animals11", "Plummeting Ursae"),
        ("chaos-blackguardzombie", "Pale Blackguard"),
        ("chaos-vultures", "Mindless Scavengers"),
        ("chaos-soldierskeletons-waterspiral", "Impure Archers"),
        ("chaos-sandleaper", "Dust Scrabblers"),
        ("chaos-sandleaper2", "Infested Skitterers"),
        ("act8sandleaper3", "Sand Leapers"),
        ("chaos-sandleaper3", "Sand Leapers"),
        ("chaos-sandleaperbulls4", "Grazing Taurus"),
        ("chaos-massskeleton-skeletons", "Voll Skeletons"),
        ("chaos-miners1", "Pocked Miners and Lanternbearers"),
        ("chaos-miners2", "Pocked Miners and Lanternbearers"),
        ("chaos-miners3", "Pocked Miners"),
        ("chaos-bats", "Shriekers"),
        ("chaos-kaomgroup1", "Kaom's Chosen"),
        ("chaos-kaomgroup2", "Hyrri's Sisters"),
        ("chaos-kaomgroup3", "Fury Hounds"),
        ("chaos-kaomgroup4", "Tukohama Warriors"),
        ("chaos-heitiki-emanant", "Inua Totems"),
        ("chaos-hounds", "Fury Hounds"),
        ("chaos-arena1", "Pitbull Demons"),
        ("chaos-arena2", "Mad Gladiators"),
        ("chaos-arena3", "Fighting Bulls"),
        ("chaos-arena4", "Shackled Hellions"),
        ("chaos-arena5", "Rooster Fiends"),
        ("chaos-belly1-emanant-abomination", "Wandering Eyes"),
        ("chaos-belly2-abomination", "Corrupted Beasts"),
        ("chaos-belly3-abomination", "Bonewarped Skeletons"),
        ("chaos-belly4-abomination", "Gut Flayers"),
        ("chaos-belly5-abomination", "Gut Spikers"),
        ("chaos-belly6-abomination", "Noisome Ophidians"),
        ("chaos-belly7-abomination", "Bloodstained Skeletons"),
        ("chaos-parasitecrabs1", "Savage Crabs"),
        ("chaos-parasitecrabs2", "Infested Crabs"),
        ("chaos-parasitecrabs3", "Infested Crawlers"),
        ("chaos-statues1", "Idolised Saints"),
        ("chaos-busts1", "Living Memorials"),
        ("chaos-cultists1", "Kitava Zealots"),
        ("chaos-cultists2", "Kitava Zealots"),
        ("chaos-cultists3", "Kitava Zealots"),
        ("chaos-cultists4", "Kitava Zealots"),
        ("chaos-cultists5", "Sanguine Sisters"),
        ("chaos-cultists6", "Kitava Zealots"),
        ("chaos-fireelementals1", "Prayer Guards"),
        ("chaos-ghostcivilians1", "Vengeful Souls"),
        ("chaos-scorpions1", "Hybrid Arachnids"),
        ("chaos-parasiticsquid1", "Swarthy Molluscs"),
        ("chaos-parasiticsquid2", "Animated Reefs "),
        ("chaos-templar1", "Oriath Enforcers"),
        ("chaos-templar2", "Temple Guardians"),
        ("chaos-templar3", "Temple Justicars"),
        ("chaos-templar4", "Flame Mothers"),
        ("chaos-templar5", "Blackguard Stalkers"),
        ("chaos-templar6", "Slave Catchers"),
        ("chaos-templar7", "Blackguard Jaegers"),
        ("chaos-templar8", "Indoctrinated Slaves"),
        ("chaos-puselementalminer", "Mephitic Maidens"),
        ("chaos-puselementalspiral", "Mephitic Maidens"),
        ("chaos-warhero", "Spectral Soldiers"),
        ("chaos-warhero2", "Spectral Legionnaires"),
        ("chaos-blackguardzombieleader", "Pale Blackguards"),
        ("chaos-gemfrogchrome", "Chrome-Infused Croakers"),
        ("chaos-iguanachrome", "Chrome-Infused Chimerals"),
        ("chaos-spikerchrome", "Chrome-Infused Goliaths"),
        ("chaos-citystalkercold", "Undying Patricians"),
        ("chaos-gargoylechrome", "Chromatic Gargoyles"),
        ("chaos-lunarisaxis1", "Lunaris Elites"),
        ("chaos-lunarisaxis2", "Lunar Scouts"),
        ("chaos-lunarisaxis3", "Lunaris Elites"),
        ("chaos-lunarisaxis4", "Lunar Scouts"),
        ("chaos-solarisaxis1", "Solaris Elites"),
        ("chaos-solarisaxis2", "Solar Scouts"),
        ("chaos-solarisaxis3", "Solaris Elites"),
        ("chaos-solarisaxis4", "Solar Scouts"),
        ("chaos-lunarisdemon1", "Night Horrors"),
        ("chaos-lunarisdemon2", "Lunar Miscreations"),
        ("chaos-lunarisdemon3", "Lunaris Concubines"),
        ("chaos-lunariscannibal1", "Lunaris Zealots"),
        ("chaos-lunariscannibal2", "Lunaris Zealots"),
        ("chaos-rats1", "Swarming Rats"),
        ("chaos-scarecrows1", "Scarecrows"),
        ("chaos-farmerskeletons1", "Brittle Farmers"),
        ("chaos-solarisfireelemental1", "Solar Elementals"),
        ("chaos-solarisfireelemental2", "Solar Elementals"),
        ("chaos-guardianssolaris1", "Assault Ribbons"),
        ("chaos-roman-skeletons1", "Brittle Gladiators"),
        ("chaos-snakespitter1", "Acid Slitherers"),
        ("chaos-solarislions1", "Auric Lions"),
        ("chaos-arcticwolf1", "Snow Wolves"),
        ("chaos-mummyscagespider", "Dessicated Maraketh"),
        ("chaos-rhex1", "Wild Rhexes"),
        ("chaos-rhex2", "Adolescent Rhexes"),
        ("chaos-scorpionsdesert1", "Black Scorpions"),
        ("chaos-scorpionsdesert2", "Sulphuric Scorpions"),
        ("chaos-gemmonsterfire", "Vaulting Croakers and Feral Chimerals"),
        ("chaos-bullfresh", "Avalanche Riders"),
        ("chaos-bleachedcrabs1", "Sulphurspawn"),
        ("chaos-mossmonsterice", "Frozen Titans"),
        ("chaos-lynx", "Mountain Lynx"),
        ("chaos-mossrockelementals", "Mountain Fragments"),
        ("chaos-wraith", "Wraiths"),
        ("chaos-paupers", "Restless Paupers"),
        ("chaos-regurgitator1", "Warped Regurgitators and Handmaidens"),
        ("chaos-blackguardinquisitorkitava1", "Depraved Justicars"),
        ("chaos-blackguardoriathkitava", "Depraved Enforcers"),
        ("chaos-slavecatcherkitava", "Depraved Slavers"),
        ("chaos-undeadmother", "Kitava Zealots"),
        ("chaos-wickermancultists", "Kitava Zealots"),
        ("chaos-marakethrider", "Rhexback Riders"),
        ("chaos-glasselementals", "Window Shards"),
        ("chaos-reliquarian", "Reliquarians"),
        ("chaos-dropbearparasite1", "Infested Tunnelfiends"),
        ("chaos-ossuaryskeleton1", "Brittle Clerics"),
        ("chaos-ossuaryskeleton2", "Desecrated Saints"),
        ("chaos-reliquaryskeleton1", "Gleaming Saints"),
        ("chaos-doedrewraiths", "Doedre's Suffering"),
        ("chaos-flamebearerchaos", "Maligaro's Cruelty"),
        ("chaos-shavronnecitystalker", "Shavronne's Envy"),
        ("chaos-citystalkerscold1", "Undying Labourers"),
        ("chaos-citystalkerslightning1", "Undying Operators"),
        ("chaos-snakeflameparasite", "Host Adders"),
        ("chaos-parasites1", "Parasites"),
        ("chaos-templarslavedriverkitava1", "Deformed Slaves"),
        ("chaos-blackguardtorturerkitava1", "Corrupted Extractors"),
        ("chaos-water-spiral1", "Seething Brine"),
        ("AtlasUber-fire1", "Cannibal Fire-Breathers and Arsonists"),
        ("AtlasUber-fire2", "Goatmen Magus"),
        ("AtlasUber-fire3", "Molten Chimerals"),
        ("AtlasUber-fire4", "Plated Arachnids"),
        ("AtlasUber-fire5", "Depraved and Deranged Slavers"),
        ("AtlasUber-fire6", "Resolute Guards"),
        ("AtlasUber-fire7", "Chosen Loyals"),
        ("AtlasUber-fire8", "Maddened Fanatics"),
        ("Atlasuber-cold1", "Chilling Hellions"),
        ("Atlasuber-cold2", "Animated Glaciers"),
        ("Atlasuber-cold3", "Reefrend"),
        ("Atlasuber-cold4", "Typhoons"),
        ("Atlasuber-cold5", "Vibrant Gargoyles"),
        ("AtlasUber-Lightning1", "Lightning Protectors"),
        ("AtlasUber-Lightning2", "Tasing Miscreations"),
        ("AtlasUber-Lightning3", "Shocking Mistresses"),
        ("AtlasUber-Lightning4", "Refinery Shockers"),
        ("AtlasUber-Lightning5", "Bearded Bolters"),
        ("AtlasUber-Lightning6", "Lightning Tamers"),
        ("AtlasUber-Chaos1", "Decayed Skeletons"),
        ("AtlasUber-Chaos2", "Melted Goliaths"),
        ("AtlasUber-Chaos3", "Noxious Crawlers and Plagued Arachnids"),
        ("AtlasUber-Chaos4", "Putrid Corpses"),
        ("AtlasUber-Chaos5", "Chaos Fanatics"),
        ("AtlasUber-Chaos6", "Spitting Parasites"),
        ("AtlasUber-Physical1", "Confession Extractors"),
        ("AtlasUber-Physical2", "Bloody Barrowers"),
        ("AtlasUber-Physical3", "Repulsive Ophidians"),
        ("AtlasUber-Physical4", "Sanguine Serpents"),
        ("AtlasUber-Physical5", "Temple Guards"),
        ("AtlasUber-Physical6", "Sandworn Poachers"),
        ("AtlasUber-Physical7", "Mindless Thralls"),
        ("AtlasUber-Physical8", "Stoneskin Dancers"),
        ("AtlasUber-Physical9", "Ancient Spearmen"),
        ("AtlasUber-Physical10", "Bloodstained Conjuror"),
        ("AtlasUber-Physical11", "Disruptors"),
        ("AtlasUber-Physical12", "Saintly Evangelists"),
        ("AtlasUber-Physical13", "Forgotten Marksmen"),
        ("AtlasUber-Physical14", "Grizzly Beasts"),
        ("AtlasUber-Physical15", "Spined Goliaths"),
        ("AtlasUber-Physical16", "Spectral Leaders and Warriors"),
        ("AtlasUber-Physical17", "Charred Beasts"),
        ("AtlasUber-Physical18", "Unresting Skeletons and Tremorbones"),
        ("AtlasUber-Physical19", "World Shatterer"),
        ("kitava-heralds-supporter", "Kitava's Heralds"),
        ("kitava-heralds-heart-supporter", "Kitava's Heralds"),
        ("bone-husks-supporter", "Bone Husks"),
        ("bone-husks-big-supporter", "Bone Husks"),
        ("breach-fire6", "Xoph's Demons"),
        ("breach-cold6", "Tul's Demons"),
        ("breach-lightning6", "Esh's Demons"),
        ("breach-physical1", "Uul-Netol's Demons"),
        ("breach-chaos1", "Chayula's Demons"),
        ("breach-generic2", "It That Watches"),
        ("breach-generic3", "It that Whispers"),
        ("InvasionBoss-Spiker", "Spined Goliaths")
    };

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<int> DevotedBonusValue { get; set; } = new RangeNode<int>(50, 0, 100);
    public RangeNode<float> HasPackLeaderValue { get; set; } = new RangeNode<float>(1.5f, -10f, 10f);

    public RangeNode<float> DangerWeight { get; set; } = new RangeNode<float>(0.5f, 0, 1);

    public ToggleNode MonsterValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModDanger { get; set; } = new ToggleNode(true);

    public ToggleNode GiveSuggestions { get; set; } = new ToggleNode(true);


    // Messy settings below - clean as you see fit.

    private string selectedModId;

    // Load last saved for both on initialization as its less confusing
    public string ModMobWeightingLastSaved { get; set; } = "";
    public string ModMobWeightingLastSelected { get; set; } = "";

    [JsonIgnore]
    public string popupAllFilter = "";

    [JsonIgnore]
    public string popupCurrentFilter = "";

    [JsonIgnore]
    private const string EditModMobListPopup = "Modify Mob list";

    [JsonIgnore]
    private const string ClearSettingPopup = "Clear Confirmation";

    [JsonIgnore]
    private const string OverwritePopup = "Overwrite Confirmation";

    public Swappable HotSwap { get; set; } = new();
    public bool isDevotionModSelected;

    public class Swappable
    {
        public Dictionary<string, Dictionary<string, float>> ModMobWeightings { get; set; } = [];
        public Dictionary<string, Dictionary<string, float>> DevotionModMobWeightings { get; set; } = [];
        public bool HideZeroSetWeights;
    }


    [JsonIgnore]
    public CustomNode HotSwappableConfiguration { get; }

    [JsonIgnore]
    public CustomNode HotSwappableWeightings { get; }

    public NecropolisQolSettings()
    {
        List<string> files;

        if (HotSwap.ModMobWeightings == null)
        {
            ClearNormalWeightings();
        }

        if (HotSwap.DevotionModMobWeightings == null)
        {
            ClearDevotionWeightings();
        }

        HotSwappableConfiguration = new CustomNode
        {
            DrawDelegate = () =>
            {
                var _fileSaveName = ModMobWeightingLastSaved;
                var _selectedFileName = ModMobWeightingLastSelected;

                if (!ImGui.CollapsingHeader(
                        $"Load / Save Hot Swappable Configurations##{NecropolisQol.Main.Name}Load / Save",
                        ImGuiTreeNodeFlags.DefaultOpen
                    ))
                {
                    return;
                }

                ImGui.Indent();
                ImGui.InputTextWithHint("##SaveAs", "File Path...", ref _fileSaveName, 100);
                ImGui.SameLine();

                if (ImGui.Button("Save To File"))
                {
                    files = GetFiles();

                    _fileSaveName = Path.GetInvalidFileNameChars().Aggregate(
                        _fileSaveName,
                        (current, c) => current.Replace(c, '_')
                    );

                    if (_fileSaveName != string.Empty)
                    {
                        if (files.Contains(_fileSaveName))
                        {
                            ImGui.OpenPopup(OverwritePopup);
                        }
                        else
                        {
                            SaveFile(HotSwap, $"{_fileSaveName}.json");
                        }
                    }
                }

                ImGui.Separator();

                if (ImGui.BeginCombo("Load File##LoadFile", _selectedFileName))
                {
                    files = GetFiles();

                    foreach (var fileName in files)
                    {
                        var isSelected = _selectedFileName == fileName;

                        if (ImGui.Selectable(fileName, isSelected))
                        {
                            _selectedFileName = fileName;
                            _fileSaveName = fileName;
                            LoadFile(fileName);
                        }

                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.Separator();

                if (ImGui.Button("Open Template Folder"))
                {
                    var configDir = NecropolisQol.Main.ConfigDirectory;
                    var directoryToOpen = Directory.Exists(configDir);

                    if (!directoryToOpen)
                    {
                        // Log error when the config directory doesn't exist
                    }

                    if (configDir != null)
                    {
                        Process.Start("explorer.exe", configDir);
                    }
                }

                if (ShowButtonPopup(OverwritePopup, ["Are you sure?", "STOP"], out var saveSelectedIndex) &&
                    saveSelectedIndex == 0)
                {
                    SaveFile(HotSwap, $"{_fileSaveName}.json");
                }

                ModMobWeightingLastSaved = _fileSaveName;
                ModMobWeightingLastSelected = _selectedFileName;
                ImGui.Unindent();
            }
        };

        string modFilter = "", mobFilter = "";

        HotSwappableWeightings = new CustomNode
        {
            DrawDelegate = () =>
            {
                if (!ImGui.CollapsingHeader("Hot Swappable Configurations"))
                {
                    return;
                }

                ImGui.Indent();

                if (!ImGui.TreeNode("Modifier & Monster Weighting"))
                {
                    return;
                }

                if (ImGui.Checkbox("Show Devotion Mods", ref isDevotionModSelected))
                {
                    selectedModId = null;
                }

                var modWeightings = isDevotionModSelected ? HotSwap.DevotionModMobWeightings : HotSwap.ModMobWeightings;
                var mods = isDevotionModSelected ? DevotionMods : NormalMods;
                selectedModId ??= mods.FirstOrDefault().Id;

                if (ImGui.Button("[x] Clear All"))
                {
                    ImGui.OpenPopup(ClearSettingPopup);
                }

                if (ShowButtonPopup(ClearSettingPopup, ["Are you sure?", "STOP"], out var clearSelectedIndex))
                {
                    if (clearSelectedIndex == 0)
                    {
                        if (isDevotionModSelected)
                        {
                            ClearDevotionWeightings();
                        }
                        else
                        {
                            ClearNormalWeightings();
                        }

                        return;
                    }
                }

                ImGui.SameLine();
                ImGui.Checkbox("Hide 0 Set Weightings", ref HotSwap.HideZeroSetWeights);
                ImGui.InputTextWithHint("Modifier Filter##ModFilter", "Filter Modifiers here", ref modFilter, 100);
                ImGui.InputTextWithHint("Monster Filter##MobFilter", "Filter Monsters here", ref mobFilter, 100);

                if (ImGui.Button($"Edit Monster List for Modifier: {GetHumanName(selectedModId, mods)}"))
                {
                    ImGui.OpenPopup(EditModMobListPopup);
                }

                ShowMonsterListPopup(EditModMobListPopup, modWeightings[selectedModId], out var outList);
                modWeightings[selectedModId] = outList;

                if (!ImGui.BeginTable("ModMobConfig", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                {
                    return;
                }

                ImGui.TableSetupColumn("Modifiers", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Monsters", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();

                // Modifiers column
                ImGui.TableNextColumn();

                var filteredMods = mods
                                   .Where(t => t.Name.Contains(modFilter, StringComparison.InvariantCultureIgnoreCase))
                                   .ToList();

                for (var i = 0; i < filteredMods.Count; i++)
                {
                    var (modId, modName) = filteredMods[i];
                    HighlightSelected(modId, modName, selectedModId == modId, () => selectedModId = modId);

                    // Add a separator for all but the last item
                    if (i < filteredMods.Count - 1)
                    {
                        ImGui.Separator();
                    }
                }

                // Monsters column
                ImGui.TableNextColumn();
                DisplayMobWeightings(mobFilter, ref modWeightings);
                ImGui.EndTable();
                ImGui.TreePop();
                ImGui.Unindent();

                if (isDevotionModSelected)
                {
                    HotSwap.DevotionModMobWeightings = modWeightings;
                }
                else
                {
                    HotSwap.ModMobWeightings = modWeightings;
                }
            }
        };
    }

    private static void HighlightSelected(string modId, string modName, bool isSelected, Action onClick)
    {
        if (ImGui.Selectable($"{modName}##{modId}", isSelected))
        {
            onClick.Invoke();
        }
    }

    private void DisplayMobWeightings(string mobFilter, ref Dictionary<string, Dictionary<string, float>> inputList)
    {
        if (string.IsNullOrEmpty(selectedModId) ||
            !inputList.TryGetValue(selectedModId, out var mobWeightings))
        {
            return;
        }

        foreach (var (mobId, weight) in mobWeightings)
        {
            var mobName = AllMonsters.FirstOrDefault(m => m.Id == mobId).Name;

            if (mobName == null || !mobName.Contains(mobFilter, StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            if (!DEFAULT_MONSTER.Equals(mobId) && HotSwap.HideZeroSetWeights && mobWeightings[mobId] == 0f)
            {
                continue;
            }

            var refModWeight = mobWeightings[mobId];
            DisplayWeightSlider(mobId, ref refModWeight, mobName, weight);
            mobWeightings[mobId] = refModWeight;
        }

        return;
    }

    private static void DisplayWeightSlider(string mobId, ref float weight, string mobName, float initialWeight)
    {
        var tempWeight = weight;
        ImGui.SliderFloat($"##{mobId}", ref tempWeight, -10.0f, 10.0f);

        if (tempWeight != initialWeight)
        {
            weight = tempWeight;
        }

        ImGui.SameLine();
        ImGui.Text(mobName);
    }
    private void ClearNormalWeightings()
    {
        HotSwap.ModMobWeightings = [];

        foreach (var (id, name) in NormalMods)
            HotSwap.ModMobWeightings[id] = [];

        selectedModId = NormalMods.FirstOrDefault().Id;
    }
    private void ClearDevotionWeightings()
    {
        HotSwap.DevotionModMobWeightings = [];

        foreach (var (id, name) in DevotionMods)
            HotSwap.DevotionModMobWeightings[id] = [];

        selectedModId = DevotionMods.FirstOrDefault().Id;
    }

    #region Save / Load Section

    public static bool ShowButtonPopup(string popupId, List<string> items, out int selectedIndex)
    {
        selectedIndex = -1;
        var isItemClicked = false;
        var showPopup = true;

        if (!ImGui.BeginPopupModal(
                popupId,
                ref showPopup,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize
            ))
        {
            return false;
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (ImGui.Button(items[i]))
            {
                selectedIndex = i;
                isItemClicked = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
        }

        ImGui.EndPopup();
        return isItemClicked;
    }

    public void SaveFile(Swappable input, string filePath)
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, filePath);
            var jsonString = JsonConvert.SerializeObject(input, Formatting.Indented);
            File.WriteAllText(fullPath, jsonString);
        }
        catch
        {
            // ignored
        }
    }

    public void LoadFile(string fileName)
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, $"{fileName}.json");
            var fileContent = File.ReadAllText(fullPath);
            HotSwap = JsonConvert.DeserializeObject<Swappable>(fileContent);
        }
        catch
        {
            // ignored
        }
    }

    public List<string> GetFiles()
    {
        var fileList = new List<string>();

        try
        {
            var dir = new DirectoryInfo(NecropolisQol.Main.ConfigDirectory);
            fileList = dir.GetFiles().Select(file => Path.GetFileNameWithoutExtension(file.Name)).ToList();
        }
        catch
        {
            // ignored
        }

        return fileList;
    }

    #endregion

    public bool ShowMonsterListPopup(string popupId, Dictionary<string, float> inputList, out Dictionary<string, float> outputList)
    {
        outputList = new Dictionary<string, float>(inputList);
        var showPopup = true;

        if (!ImGui.BeginPopupModal(popupId, ref showPopup, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse))
        {
            return false;
        }

        if (ImGui.Button("Close"))
        {
            ImGui.CloseCurrentPopup();
        }

        if (!ImGui.BeginTable("ModMobConfig", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
        {
            return false;
        }

        ImGui.TableSetupColumn("All Monsters", ImGuiTableColumnFlags.None, 500);
        ImGui.TableSetupColumn("Current Monsters", ImGuiTableColumnFlags.None, 500);
        ImGui.TableHeadersRow();

        // Column for unselected monsters
        ImGui.TableNextColumn();
        ImGui.InputTextWithHint("##AllMonsters", "Filter..", ref popupAllFilter, 100);

        foreach (var monster in AllMonsters)
        {
            if (!string.IsNullOrEmpty(popupAllFilter) && !monster.Name.Contains(popupAllFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (inputList.ContainsKey(monster.Id))
            {
                continue;
            }

            if (ImGui.Selectable($"{GetHumanName(monster.Id, AllMonsters)}##{monster.Id}", false, ImGuiSelectableFlags.DontClosePopups))
            {
                inputList[monster.Id] = 0.0f;
            }
        }


        // Column for selected monsters
        ImGui.TableNextColumn();
        ImGui.InputTextWithHint("##CurrentMonsters", "Filter..", ref popupCurrentFilter, 100);

        foreach (var kvp in inputList)
        {
            if (!string.IsNullOrEmpty(popupCurrentFilter) && !GetHumanName(kvp.Key, AllMonsters).Contains(popupCurrentFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (ImGui.Selectable($"{GetHumanName(kvp.Key, AllMonsters)}##{kvp.Key}", false, ImGuiSelectableFlags.DontClosePopups))
            {
                inputList.Remove(kvp.Key);
            }
        }

        ImGui.EndTable();
        ImGui.EndPopup();
        outputList = inputList;
        return !showPopup;
    }

    public string GetHumanName(string id, IReadOnlyList<(string Id, string Name)> list) =>
        list.FirstOrDefault(monster => monster.Id == id).Name ?? "<Unknown_Name>";

}
