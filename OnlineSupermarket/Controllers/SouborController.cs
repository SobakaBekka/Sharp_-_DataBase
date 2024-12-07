using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using OnlineSupermarket.Models;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace OnlineSupermarket.Controllers
{
    public class SouborController : Controller
    {
        private readonly string _connectionString;
        private readonly ILogger<SouborController> _logger;

        public SouborController(IConfiguration configuration, ILogger<SouborController> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult Index()
        {
            List<Soubor> soubory = new List<Soubor>();
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ZOBRAZ_VSECHNY_SOUBORY", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    OracleParameter cursorParam = cmd.Parameters.Add("p_CURSOR", OracleDbType.RefCursor);
                    cursorParam.Direction = ParameterDirection.Output;

                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            soubory.Add(new Soubor
                            {
                                IDSOUBORU = reader.GetInt32(reader.GetOrdinal("IDSOUBORU")),
                                NAZEV_SOUBORU = reader.GetString(reader.GetOrdinal("NAZEV_SOUBORU")),
                                TYP_SOUBORU = reader.GetString(reader.GetOrdinal("TYP_SOUBORU")),
                                PRIPONA_SOUBORU = reader.GetString(reader.GetOrdinal("PRIPONA_SOUBORU")),
                                DATUM_NAHRANI = reader.GetDateTime(reader.GetOrdinal("DATUM_NAHRANI")),
                                DATUM_MODIFIKACE = reader.IsDBNull(reader.GetOrdinal("DATUM_MODIFIKACE")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("DATUM_MODIFIKACE")),
                                OBSAH = (byte[])reader["OBSAH"],
                                ID_REGISUZIVATELU = reader.IsDBNull(reader.GetOrdinal("ID_REGISUZIVATELU")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ID_REGISUZIVATELU"))
                            });
                        }
                    }
                }
            }
            return View(soubory);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Soubor soubor, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    soubor.OBSAH = memoryStream.ToArray();
                    soubor.NAZEV_SOUBORU = file.FileName;
                    soubor.TYP_SOUBORU = file.ContentType;
                    soubor.PRIPONA_SOUBORU = Path.GetExtension(file.FileName);
                    soubor.DATUM_MODIFIKACE = DateTime.Now;
                }
            }

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("VLOZ_SOUBOR", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_NAZEV_SOUBORU", OracleDbType.Varchar2).Value = soubor.NAZEV_SOUBORU;
                    cmd.Parameters.Add("p_TYP_SOUBORU", OracleDbType.Varchar2).Value = soubor.TYP_SOUBORU;
                    cmd.Parameters.Add("p_PRIPONA_SOUBORU", OracleDbType.Varchar2).Value = soubor.PRIPONA_SOUBORU;
                    cmd.Parameters.Add("p_DATUM_MODIFIKACE", OracleDbType.Date).Value = soubor.DATUM_MODIFIKACE;
                    cmd.Parameters.Add("p_OBSAH", OracleDbType.Blob).Value = soubor.OBSAH;
                    cmd.Parameters.Add("p_ID_REGISUZIVATELU", OracleDbType.Int32).Value = soubor.ID_REGISUZIVATELU;
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            try
            {
                Soubor? soubor = null;
                using (OracleConnection conn = new OracleConnection(_connectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = new OracleCommand("ZOBRAZ_SOUBOR", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("p_IDSOUBORU", OracleDbType.Int32).Value = id;
                        cmd.Parameters.Add("p_NAZEV_SOUBORU", OracleDbType.Varchar2, 255).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_TYP_SOUBORU", OracleDbType.Varchar2, 255).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_PRIPONA_SOUBORU", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_DATUM_NAHRANI", OracleDbType.Date).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_DATUM_MODIFIKACE", OracleDbType.Date).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_OBSAH", OracleDbType.Blob).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("p_ID_REGISUZIVATELU", OracleDbType.Int32).Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        soubor = new Soubor
                        {
                            IDSOUBORU = id,
                            NAZEV_SOUBORU = cmd.Parameters["p_NAZEV_SOUBORU"].Value?.ToString() ?? string.Empty,
                            TYP_SOUBORU = cmd.Parameters["p_TYP_SOUBORU"].Value?.ToString() ?? string.Empty,
                            PRIPONA_SOUBORU = cmd.Parameters["p_PRIPONA_SOUBORU"].Value?.ToString() ?? string.Empty,
                            DATUM_NAHRANI = ((Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_DATUM_NAHRANI"].Value).Value,
                            DATUM_MODIFIKACE = cmd.Parameters["p_DATUM_MODIFIKACE"].Value == DBNull.Value ? (DateTime?)null : ((Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_DATUM_MODIFIKACE"].Value).Value,
                            OBSAH = ConvertOracleBlobToByteArray(cmd.Parameters["p_OBSAH"].Value as Oracle.ManagedDataAccess.Types.OracleBlob) ?? Array.Empty<byte>(),
                            ID_REGISUZIVATELU = cmd.Parameters["p_ID_REGISUZIVATELU"].Value == DBNull.Value ? (int?)null : ConvertOracleDecimalToInt32((Oracle.ManagedDataAccess.Types.OracleDecimal?)cmd.Parameters["p_ID_REGISUZIVATELU"].Value)
                        };
                    }
                }
                return View(soubor);
            }
            catch (OracleException ex)
            {
                _logger.LogError(ex, "OracleException occurred while editing soubor with ID: {id}", id);
                return StatusCode(500, "Internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while editing soubor with ID: {id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public IActionResult Edit(Soubor soubor, IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    soubor.OBSAH = memoryStream.ToArray();
                    soubor.NAZEV_SOUBORU = file.FileName;
                    soubor.TYP_SOUBORU = file.ContentType;
                    soubor.PRIPONA_SOUBORU = Path.GetExtension(file.FileName);
                    soubor.DATUM_MODIFIKACE = DateTime.Now;
                }
            }

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("AKTUALIZUJ_SOUBOR", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_IDSOUBORU", OracleDbType.Int32).Value = soubor.IDSOUBORU;
                    cmd.Parameters.Add("p_NAZEV_SOUBORU", OracleDbType.Varchar2).Value = soubor.NAZEV_SOUBORU;
                    cmd.Parameters.Add("p_TYP_SOUBORU", OracleDbType.Varchar2).Value = soubor.TYP_SOUBORU;
                    cmd.Parameters.Add("p_PRIPONA_SOUBORU", OracleDbType.Varchar2).Value = soubor.PRIPONA_SOUBORU;
                    cmd.Parameters.Add("p_DATUM_MODIFIKACE", OracleDbType.Date).Value = soubor.DATUM_MODIFIKACE;
                    cmd.Parameters.Add("p_OBSAH", OracleDbType.Blob).Value = soubor.OBSAH;
                    cmd.Parameters.Add("p_ID_REGISUZIVATELU", OracleDbType.Int32).Value = soubor.ID_REGISUZIVATELU;
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("SMAZ_SOUBOR", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_IDSOUBORU", OracleDbType.Int32).Value = id;
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ViewFile(int id)
        {
            Soubor? soubor = null;
            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand("ZOBRAZ_SOUBOR", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("p_IDSOUBORU", OracleDbType.Int32).Value = id;
                    cmd.Parameters.Add("p_NAZEV_SOUBORU", OracleDbType.Varchar2, 255).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_TYP_SOUBORU", OracleDbType.Varchar2, 255).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_PRIPONA_SOUBORU", OracleDbType.Varchar2, 10).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_DATUM_NAHRANI", OracleDbType.Date).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_DATUM_MODIFIKACE", OracleDbType.Date).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_OBSAH", OracleDbType.Blob).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("p_ID_REGISUZIVATELU", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    soubor = new Soubor
                    {
                        IDSOUBORU = id,
                        NAZEV_SOUBORU = cmd.Parameters["p_NAZEV_SOUBORU"].Value?.ToString() ?? string.Empty,
                        TYP_SOUBORU = cmd.Parameters["p_TYP_SOUBORU"].Value?.ToString() ?? string.Empty,
                        PRIPONA_SOUBORU = cmd.Parameters["p_PRIPONA_SOUBORU"].Value?.ToString() ?? string.Empty,
                        DATUM_NAHRANI = ((Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_DATUM_NAHRANI"].Value).Value,
                        DATUM_MODIFIKACE = cmd.Parameters["p_DATUM_MODIFIKACE"].Value == DBNull.Value ? (DateTime?)null : ((Oracle.ManagedDataAccess.Types.OracleDate)cmd.Parameters["p_DATUM_MODIFIKACE"].Value).Value,
                        OBSAH = ConvertOracleBlobToByteArray(cmd.Parameters["p_OBSAH"].Value as Oracle.ManagedDataAccess.Types.OracleBlob) ?? Array.Empty<byte>(),
                        ID_REGISUZIVATELU = cmd.Parameters["p_ID_REGISUZIVATELU"].Value == DBNull.Value ? (int?)null : ConvertOracleDecimalToInt32((Oracle.ManagedDataAccess.Types.OracleDecimal?)cmd.Parameters["p_ID_REGISUZIVATELU"].Value)
                    };
                }
            }

            if (soubor == null || soubor.OBSAH == null || soubor.OBSAH.Length == 0)
            {
                return NotFound();
            }

            Response.Headers.Add("Content-Disposition", $"inline; filename={soubor.NAZEV_SOUBORU}");
            return File(soubor.OBSAH, soubor.TYP_SOUBORU);
        }

        private byte[]? ConvertOracleBlobToByteArray(Oracle.ManagedDataAccess.Types.OracleBlob? oracleBlob)
        {
            if (oracleBlob == null || oracleBlob.IsNull)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                oracleBlob.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private int? ConvertOracleDecimalToInt32(Oracle.ManagedDataAccess.Types.OracleDecimal? oracleDecimal)
        {
            if (!oracleDecimal.HasValue || oracleDecimal.Value.IsNull)
            {
                return null;
            }

            return oracleDecimal.Value.ToInt32();
        }
    }
}