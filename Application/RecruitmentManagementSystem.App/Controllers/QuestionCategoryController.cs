﻿using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using RecruitmentManagementSystem.App.ViewModels.Question;
using RecruitmentManagementSystem.Data.Interfaces;
using RecruitmentManagementSystem.Model;
using JsonResult = RecruitmentManagementSystem.App.Infrastructure.ActionResults.JsonResult;

namespace RecruitmentManagementSystem.App.Controllers
{
    [Authorize]
    public class QuestionCategoryController : BaseController
    {
        private readonly IQuestionCategoryRepository _questionCategoryRepository;

        public QuestionCategoryController(IQuestionCategoryRepository questionCategoryRepository)
        {
            _questionCategoryRepository = questionCategoryRepository;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _questionCategoryRepository.FindAll().ProjectTo<QuestionCategoryModel>();

            if (Request.IsAjaxRequest())
            {
                return new JsonResult(model, JsonRequestBehavior.AllowGet);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionCategoryModel question)
        {
            if (!ModelState.IsValid) return View(question);

            _questionCategoryRepository.Insert(new QuestionCategory
            {
                Name = question.Name,
                Description = question.Description,
                CreatedBy = User.Identity.GetUserId(),
                UpdatedBy = User.Identity.GetUserId()
            });

            _questionCategoryRepository.Save();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            var model =
                _questionCategoryRepository.FindAll()
                    .ProjectTo<QuestionCategoryModel>()
                    .FirstOrDefault(x => x.Id == id);

            if (model == null) return new HttpNotFoundResult();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var model =
                _questionCategoryRepository.FindAll()
                    .ProjectTo<QuestionCategoryModel>()
                    .FirstOrDefault(x => x.Id == id);

            if (model == null) return new HttpNotFoundResult();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionCategoryModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _questionCategoryRepository.Update(new QuestionCategory
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            });

            _questionCategoryRepository.Save();
            return RedirectToAction("Index");
        }
    }
}