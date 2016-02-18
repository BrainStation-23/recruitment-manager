﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Identity;
using RecruitmentManagementSystem.App.Infrastructure.Helpers;
using RecruitmentManagementSystem.Core.Constants;
using RecruitmentManagementSystem.Core.Models.Question;
using RecruitmentManagementSystem.Data.Interfaces;
using RecruitmentManagementSystem.Model;
using File = RecruitmentManagementSystem.Model.File;
using JsonResult = RecruitmentManagementSystem.App.Infrastructure.ActionResults.JsonResult;
using RecruitmentManagementSystem.Core.Interfaces;
using RecruitmentManagementSystem.Core.Mappings;

namespace RecruitmentManagementSystem.App.Controllers
{
    [Authorize]
    public class QuestionController : BaseController
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IQuestionService _questionService;
        private readonly IModelFactory _modelFactory;

        public QuestionController(IQuestionRepository questionRepository, IFileRepository fileRepository,
            IChoiceRepository choiceRepository, IQuestionService questionService, IModelFactory modelFactory)
        {
            _questionRepository = questionRepository;
            _fileRepository = fileRepository;
            _choiceRepository = choiceRepository;
            _questionService = questionService;
            _modelFactory = modelFactory;
        }

        [HttpGet]
        public ActionResult List()
        {
            var model = _questionService.GetPagedList();

            if (Request.IsAjaxRequest())
            {
                return new JsonResult(model, JsonRequestBehavior.AllowGet);
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            var viewModel =
                _questionRepository.FindAll().ProjectTo<QuestionModel>().SingleOrDefault(x => x.Id == id);

            if (Request.IsAjaxRequest())
            {
                return new JsonResult(viewModel, JsonRequestBehavior.AllowGet);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return new JsonResult(ModelState.Values.SelectMany(v => v.Errors));
            }

            _questionService.Insert(model);

            return Json(null);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var viewModel = _questionRepository.FindAll()
                .ProjectTo<QuestionModel>()
                .SingleOrDefault(x => x.Id == id);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuestionCreateModel model)
        {
            var entity = _questionRepository.Find(model.Id);

            if (entity == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                ModelState.AddModelError("", "Question not found.");
                return new JsonResult(ModelState.Values.SelectMany(v => v.Errors));
            }

            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return new JsonResult(ModelState.Values.SelectMany(v => v.Errors));
            }

            _questionService.Update(model);

            return Json(null);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            var question = _questionRepository.Find(x => x.Id == id);
            if (question == null) return new HttpNotFoundResult();

            var viewModel =
                _questionRepository.FindAll().ProjectTo<QuestionModel>().SingleOrDefault(x => x.Id == id);

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _questionRepository.Delete(_questionRepository.Find(x => x.Id == id));
            _questionRepository.Save();

            return RedirectToAction("List");
        }

        #region Private Methods

        private ICollection<File> ManageFiles(HttpFileCollectionBase fileCollection)
        {
            var files = new List<File>();

            for (var index = 0; index < fileCollection.Count; index++)
            {
                if (fileCollection[index] == null || fileCollection[index].ContentLength <= 0)
                {
                    continue;
                }

                var uploadConfig = UploadFile(Request.Files[index]);

                if (uploadConfig.FileBase == null) continue;

                var file = new File
                {
                    Name = Path.GetFileName(fileCollection[index].FileName),
                    MimeType = uploadConfig.FileBase.ContentType,
                    Size = uploadConfig.FileBase.ContentLength,
                    RelativePath = uploadConfig.FilePath + uploadConfig.FileName,
                    FileType = FileType.Document,
                    CreatedBy = User.Identity.GetUserId(),
                    UpdatedBy = User.Identity.GetUserId()
                };

                files.Add(file);
            }

            return files;
        }

        private static UploadConfig UploadFile(HttpPostedFileBase fileBase)
        {
            var fileName = string.Format("{0}.{1}", Guid.NewGuid(), Path.GetFileName(fileBase.FileName));

            const string filePath = FilePath.DocumentRelativePath;

            var uploadConfig = new UploadConfig
            {
                FileBase = fileBase,
                FileName = fileName,
                FilePath = filePath
            };

            try
            {
                FileHelper.SaveFile(uploadConfig);
            }
            catch (Exception)
            {
                return new UploadConfig();
            }

            return uploadConfig;
        }


        #endregion
    }
}