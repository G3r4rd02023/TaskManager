using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Data.Entities;
using TaskManager.Helpers;

namespace TaskManager.Controllers
{
    [Authorize]
    public class TareasController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public TareasController(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }


        public async Task<IActionResult> Index()
        {                    
             var tareas = await _context.Tareas
             .ToListAsync();
             return View(tareas);                    
        }

        public async Task<IActionResult> MyTasks()
        {
            var tareasUsuario = await _context.Tareas
                .Include(t => t.Usuario)
                .Where(t => t.Usuario.UserName == User.Identity.Name)
                .ToListAsync();

            return View(tareasUsuario);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tareas == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas
                .Include(t => t.Usuario)
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
            Usuario user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                tarea.Usuario = user;
                _context.Add(tarea);
                await _context.SaveChangesAsync();
                if (user.UserType.Equals("Admin"))
                {
                   return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(MyTasks));
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
