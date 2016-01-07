﻿using System.Linq;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using RecruitmentManagementSystem.Data.Interfaces;
using JsonResult = RecruitmentManagementSystem.App.Infrastructure.ActionResults.JsonResult;
using QuestionCategory = RecruitmentManagementSystem.Core.Models.Question.QuestionCategory;
using RecruitmentManagementSystem.Core.Interfaces;

namespace RecruitmentManagementSystem.App.Controllers
{
    [Authorize]
    public class QuestionCategoryController : BaseController
    {
        private readonly IQuestionCategoryRepository _questionCategoryRepository;

        private readonly IQuestionCategoryService _questionCategoryService;

        public QuestionCategoryController(IQuestionCategoryRepository questionCategoryRepository, IQuestionCategoryService questionCategoryService)
        {
            _questionCategoryRepository = questionCategoryRepository;

            _questionCategoryService = questionCategoryService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = _questionCategoryService.GetPagedList();

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
        public ActionResult Create(QuestionCategory question)
        {
            if (!ModelState.IsValid) return View(question);

            _questionCategoryRepository.Insert(new Model.QuestionCategory
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
                    .ProjectTo<QuestionCategory>()
                    .FirstOrDefault(x => x.Id == id);

            if (model == null) return new HttpNotFoundResult();

            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            var model =
                _questionCategoryRepository.FindAll()
                    .ProjectTo<QuestionCategory>()
                    .FirstOrDefault(x => x.Id == id);

            if (model == null) return new HttpNotFoundResult();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionCategory model)
        {
            if (!ModelState.IsValid) return View(model);

            _questionCategoryRepository.Update(new Model.QuestionCategory
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