using Microsoft.AspNetCore.Mvc;
using OnlineSupermarket.Models;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace OnlineSupermarket.Controllers
{
    public class ZboziController : Controller
    {
        private readonly DatabaseHelper _dbHelper;

        public ZboziController(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // GET: Zbozi
        public IActionResult Index()
        {
            try
            {
                string sql = "SELECT * FROM ZBOZI";
                var dataTable = _dbHelper.ExecuteQuery(sql);
                var zboziList = _dbHelper.MapZbozi(dataTable);
                return View(zboziList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading data: " + ex.Message;
                return View(new List<Zbozi>()); // Return empty list on error
            }
        }


        // GET: Zbozi/Create
        public IActionResult Create()
        {
            return View(new Zbozi());
        }

        // POST: Zbozi/Create
        [HttpPost]
        public IActionResult Create(Zbozi zbozi)
        {
            if (!ModelState.IsValid)
            {
                return View(zbozi);
            }

            try
            {
                string sql = "INSERT INTO ZBOZI (NAZEV, AKTUALNICENA, CENAZEKLUBKARTOU, HMOTNOST, SLOZENI, KATEGORIE_IDKATEGORII) " +
                             "VALUES (:Nazev, :AktualniCena, :CenaZeKlubKartou, :Hmotnost, :Slozeni, :KategorieIdKategorii)";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = zbozi.Nazev },
                    new OracleParameter("AktualniCena", OracleDbType.Decimal) { Value = zbozi.AktualniCena },
                    new OracleParameter("CenaZeKlubKartou", OracleDbType.Decimal) { Value = (object)zbozi.CenaZeKlubKartou ?? DBNull.Value },
                    new OracleParameter("Hmotnost", OracleDbType.Decimal) { Value = zbozi.Hmotnost },
                    new OracleParameter("Slozeni", OracleDbType.Varchar2) { Value = zbozi.Slozeni },
                    new OracleParameter("KategorieIdKategorii", OracleDbType.Int32) { Value = zbozi.KategorieIdKategorii }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error creating record: " + ex.Message;
                return View(zbozi);
            }
        }

        // GET: Zbozi/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                string sql = "SELECT * FROM ZBOZI WHERE IDZBOZI = :IdZbozi";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdZbozi", OracleDbType.Int32) { Value = id }
                };

                var dataTable = _dbHelper.ExecuteQuery(sql, parameters);
                var zboziList = _dbHelper.MapZbozi(dataTable);

                if (zboziList.Count == 0)
                {
                    return NotFound();
                }

                return View(zboziList[0]);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading record: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Zbozi/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, Zbozi zbozi)
        {
            if (!ModelState.IsValid)
            {
                return View(zbozi);
            }

            try
            {
                string sql = "UPDATE ZBOZI SET NAZEV = :Nazev, AKTUALNICENA = :AktualniCena, " +
                             "CENAZEKLUBKARTOU = :CenaZeKlubKartou, HMOTNOST = :Hmotnost, " +
                             "SLOZENI = :Slozeni, KATEGORIE_IDKATEGORII = :KategorieIdKategorii " +
                             "WHERE IDZBOZI = :IdZbozi";

                var parameters = new OracleParameter[]
                {
                    new OracleParameter("Nazev", OracleDbType.Varchar2) { Value = zbozi.Nazev },
                    new OracleParameter("AktualniCena", OracleDbType.Decimal) { Value = zbozi.AktualniCena },
                    new OracleParameter("CenaZeKlubKartou", OracleDbType.Decimal) { Value = (object)zbozi.CenaZeKlubKartou ?? DBNull.Value },
                    new OracleParameter("Hmotnost", OracleDbType.Decimal) { Value = zbozi.Hmotnost },
                    new OracleParameter("Slozeni", OracleDbType.Varchar2) { Value = zbozi.Slozeni },
                    new OracleParameter("KategorieIdKategorii", OracleDbType.Int32) { Value = zbozi.KategorieIdKategorii },
                    new OracleParameter("IdZbozi", OracleDbType.Int32) { Value = id }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error updating record: " + ex.Message;
                return View(zbozi);
            }
        }

        // GET: Zbozi/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                string sql = "DELETE FROM ZBOZI WHERE IDZBOZI = :IdZbozi";
                var parameters = new OracleParameter[]
                {
                    new OracleParameter("IdZbozi", OracleDbType.Int32) { Value = id }
                };

                _dbHelper.ExecuteNonQuery(sql, parameters);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error deleting record: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
