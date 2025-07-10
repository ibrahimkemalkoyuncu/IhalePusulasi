using Mesfel.Data;
using Mesfel.Services;
using Microsoft.AspNetCore.Mvc;
// ...

public class AuthController : Controller
{
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ApplicationDbContext _context;

    public AuthController(IPasswordHasherService passwordHasher, ApplicationDbContext context)
    {
        _passwordHasher = passwordHasher;
        _context = context;
    }

    public IActionResult Login(string kullaniciAdi, string sifre)
    {
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi);

        if (kullanici == null || !_passwordHasher.VerifyPassword(kullanici.SifreHash, sifre))
        {
            // Hatalı giriş
            return Unauthorized();
        }

        // Başarılı giriş
        return Ok();
    }
}