using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Entities;

namespace TaskManager.Controllers
{
    public class TareasController : Controller
    {
        private readonly DataContext _context;

        public TareasController(DataContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.Tareas.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FindAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Tarea tarea)
        {
            if (id != tarea.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tarea);
                    await _context.SaveChangesAsync();
                }

                catch (Exception exception)
                {
                    ViewBag.ErrorMessage = "Ocurrió un error al actualizar la tarea: " + exception.Message;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tarea);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            try
            {
                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();


            }
            catch (Exception exception)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al eliminar la tarea: " + exception.Message;
            }

            return RedirectToAction(nameof(Index));
        }



    }
}
