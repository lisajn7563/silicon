﻿using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : Controller
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    #region SignIn
    [HttpGet]
    [Route("/login")]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> SignIn(SignInViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, viewModel.Password, viewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Home", "default");
                }
            }
        }

        ViewData["StatusMessage"] = "Incorrect email or password";
        return View(viewModel);
    }
    #endregion


    #region SignUp
    [HttpGet]
    [Route("/register")]
    public IActionResult SignUp()
    {
        return View();
    }
    [HttpPost]
    [Route("/register")]
    public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            if (!await _userManager.Users.AnyAsync(x => x.Email == viewModel.Email))
            {
                var userEntity = new UserEntity
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    UserName = viewModel.Email
                };

                var result = await _userManager.CreateAsync(userEntity, viewModel.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SignIn", "Auth");
                }
                else
                {
                    ViewData["StatusMessage"] = "Something went wrong, Please try again";

                }
            }
            else
            {
                ViewData["StatusMessage"] = "User with the same email address already exists";
            }
        }

        return View(viewModel);
    }
    #endregion

    #region Sign Out

    [HttpGet]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("SignIn", "Auth");
    }

    #endregion
}
