using CrudOperations;
using Master.Domain.Master;
using Master.Domain.Models;

namespace Master.Application.Interfaces;

public interface IMasterRepository
{
    Task<ResponseList<DepartmentDetailsModel>> GetDepartmentDetails();
    Task<ResponseList<DepartmentAdvertisementModel>> GetDepartmentAdvertisements(int departmentId);
    Task<ResponseList<DepartmentPostModel>> GetDepartmentPosts(int departmentId, int advertisementId);

    Task<ResponseList<GeneralWrittenExamCenterModel>> GetGeneralWrittenExamCenters(GeneralWrittenExamCenterRequest request);
    Task<Response> SaveAdvertisementPost(DtoAdvertisementPost dtoAdvertisementPost);
    Task<Response> UpdateAdvertisementPost(DtoAdvertisementPost dtoAdvertisementPost);
    Task<ResponseList<AdvertisementPostModel>> GetAdvertisementPostList(DtoAdvertisementPostList request);
    Task<ResponseList<PostMasterModel>> GetDepartmentPosts();
    Task<Response<AdvertisementPostModel>> GetAdvertisementPostById(int advertisementPostId);
    Task<ResponseList<DtoMenuItem>> GetMenuByRoleId(int roleId);
    Task<ResponseList<DtoRoleItem>> GetRoleList();
    Task<ResponseList<DepartmentCandidatePostModel>> GetDepartmentCandidatePostDetails(long userId, int departmentId, int advertisementId);
    Task<Response> SaveDepartment(DtoDepartment dtoDepartment);
    Task<Response> UpdateDepartment(DtoDepartment dtoDepartment);
    Task<Response<DepartmentModel>> GetDepartmentById(int departmentId);
    Task<ResponseList<DepartmentModel>> GetDepartmentList(DtoDepartmentList request);
    Task<ResponseList<DepartmentModel>> GetActiveDepartmentList();
    Task<Response> SavePost(DtoPost dtoPost);
    Task<Response> UpdatePost(DtoPost dtoPost);
    Task<Response<DtoPost>> GetPostById(int postId);
    Task<ResponseList<DtoPost>> GetPostList(DtoPostList request);
    Task<Response> SaveHelpDetails(DtoHelpDetails dtoHelpDetails);
    Task<Response<DtoHelpDetails>> GetHelpDetailsById(int helpDetailsId);
    Task<ResponseList<DtoHelpDetails>> GetHelpDetailsList();
    Task<ResponseList<DtoHelpDetails>> GetHelpDetailsByDepartment(int departmentId);
    Task<Response> SaveNotification(DtoNotification dtoNotification);
    Task<ResponseList<NotificationModel>> GetNotificationList(DtoNotificationList request);
    Task<Response<NotificationModel>> GetNotificationDetails(int srNo, int departmentId);
    Task<ResponseList<NotificationModel>> GetNotification(int departmentId);
    Task<ResponseList<LatestNewsModel>> GetLatestNews();
    Task<Response<HomeCountDetailsModel>> GetHomeCountDetails();
    Task<ResponseList<CategoryMasterModel>> GetCategory(int? categoryId);
    Task<Response> SaveFeesDetails(List<DtoFeesMasterDetailsPost> dtoFeesMasterDetailsPost);
    Task<ResponseList<FeesMasterDEtailsModel>> GetFeesMasterDetailsList(DtoFeesMasterList request);
    Task<Response> UpdateFeesMasterDetails(DtoFeesMasterDetailsPost dtoFeesMasterDetailsPost);
    Task<Response> SaveAgeCriteria(DtoAgeCriteria dtoAgeCriteria);
    Task<Response> UpdateAgeCriteria(DtoAgeCriteria dtoAgeCriteria);
    Task<Response<AgeCriteriaModel>> GetAgeCriteriaById(long ageCriteriaId);
    Task<ResponseList<AgeCriteriaModel>> GetAgeCriteriaList(DtoAgeCriteriaList request);
    Task<byte[]> GetAgeCriteriaTemplate();
    Task<ResponseList<AgeCriteriaImportErrorModel>> ImportAgeCriteria(System.Data.DataTable rows, long createdBy);

    Task<Response> SaveAdvertisementPdf(DtoAdvertisementPdf dtoAdvertisementPdf);
    Task<Response<AdvertisementPdfModel>> GetAdvertisementPdfById(long advertisementPdfId);
    Task<ResponseList<AdvertisementPdfModel>> GetAdvertisementPdfList(DtoAdvertisementPdfList request);
    Task<Response<AdvertisementPdfFileModel>> GetAdvertisementPdfFile(long advertisementPdfId);
    Task<ResponseList<AdvertisementPdfModel>> GetPublicAdvertisementPdfList(DtoPublicAdvertisementPdfList request);
    Task<Response<AdvertisementPdfFileModel>> GetPublicAdvertisementPdfFile(long advertisementPdfId);

    Task<Response> SaveRulePdf(DtoRulePdf dtoRulePdf);
    Task<Response<RulePdfModel>> GetRulePdfById(long ruleId);
    Task<ResponseList<RulePdfModel>> GetRulePdfList(DtoRulePdfList request);
    Task<Response<RulePdfFileModel>> GetRulePdfFile(long ruleId);
    Task<ResponseList<RulePdfModel>> GetPublicRulePdfList(DtoPublicRulePdfList request);
    Task<Response<RulePdfFileModel>> GetPublicRulePdfFile(long ruleId);

    Task<Response> SaveQualificationCriteria(DtoQualificationCriteriaPost dtoQualificationCriteriaPost);
    Task<Response> UpdateQualificationCriteria(DtoQualificationCriteriaPost dtoQualificationCriteriaPost);
    Task<ResponseList<QualificationCriteriaModel>> GetQualificationCriteriaList(DtoQualificationCriteriaList request);
    Task<ResponseList<QualificationCriteriaDetailModel>> GetQualificationCriteriaByPost(int departmentId, int advertisementId, int postId);


    // added by valmiki 13082026 for Dropdown
    #region Dropdown Valmiki
    Task<ResponseList<DDlList>> GetState();
    Task<ResponseList<DDlList>> GetDomicileState();
    Task<ResponseList<DDlList>> GetParallelReservation();
    Task<ResponseList<DDlList>> GetUserParallelReservation(long userid);

    Task<ResponseList<DDlList>> GetDistrict(int stateid);
    Task<ResponseList<DDlList>> GetDomicileDistrict(int stateid);
    Task<ResponseList<DDlList>> GetTaluka(int district);
    Task<ResponseList<DDlList>> GetVillage(int taluka);

    Task<ResponseList<DDlList>> GetDegree();
    Task<ResponseList<DDlList>> GetResult();
    Task<ResponseList<DDlList>> GetCourse(int degreeId);
    Task<ResponseList<DDlList>> GetSubject(int courseId);
    Task<ResponseList<DDlList>> GetBoardUniversity(int stateId);
    Task<ResponseList<DDlList>> GetQualificationMode();

    Task<ResponseList<DDlList>> GetGender();
    Task<ResponseList<DDlList>> GetMaritalStatus();
    Task<ResponseList<DDlList>> GetMotherTongue();
    Task<ResponseList<DDlList>> GetNationality();
    Task<ResponseList<DDlList>> GetDisabilityType();
    Task<ResponseList<DDlList>> GetDisabilitySubType(int disabilityTypeId);
    Task<ResponseList<DDlList>> GetDisabilityCertificateType();
    Task<ResponseList<DDlList>> GetAnathType();

    Task<ResponseList<UnitDDlList>> GetUnits(int postid);
    #endregion




    // ================= Advertisement Master =================
    Task<Response> SaveAdvertisement( DtoAdvertisement dtoAdvertisement);

    Task<Response> UpdateAdvertisement( DtoAdvertisement dtoAdvertisement);

    Task<ResponseList<AdvertisementMasterModel>> GetAdvertisementList( DtoAdvertisementList request);

    Task<Response<AdvertisementMasterModel>> GetAdvertisementById( int advertisementId);

    // ================= Advertisement Tables Master =================
    Task<Response> CloneAdvertisementTables(DtoCloneAdvertisementTables request, long createdBy);

    Task<ResponseList<AdvertisementTablesModel>> GetAdvertisementTablesList(DtoAdvertisementTablesList request);

    Task<ResponseList<DtoSourceAdvertisementTableDropdown>> GetSourceAdvertisementTables();

    Task<Response> ToggleAdvertisementTableStatus(DtoToggleAdvertisementTableStatus request);

    // ================= Department User Master =================
    Task<ResponseList<UnitModel>> GetActiveUnitList();

    Task<Response> SaveDepartmentUser(DtoDepartmentUserSave request);

    Task<ResponseList<DepartmentUserModel>> GetDepartmentUserList(DtoDepartmentUserList request);
 
    Task<Response<DepartmentUserModel>> GetDepartmentUserById(long userId);

    Task<Response> ToggleDepartmentUserStatus(long userId, bool isActive, long updatedBy);
 
    // ================= Education Master (Qualification, Course, Subject) =================
    Task<Response> SaveQualification(DtoQualification dtoQualification);
    Task<ResponseList<QualificationMasterModel>> GetQualificationMasterList(DtoQualificationList request);
    Task<Response<QualificationMasterModel>> GetQualificationById(int qualificationId);
    Task<Response> ToggleQualificationStatus(DtoQualificationStatusToggle request);

    Task<Response> SaveCourse(DtoCourse dtoCourse);
    Task<ResponseList<CourseMasterModel>> GetCourseMasterList(DtoCourseList request);
    Task<Response<CourseMasterModel>> GetCourseById(int courseId);
    Task<Response> ToggleCourseStatus(DtoCourseStatusToggle request);

    Task<Response> SaveSubject(DtoSubject dtoSubject);
    Task<ResponseList<SubjectMasterModel>> GetSubjectMasterList(DtoSubjectList request);
    Task<Response<SubjectMasterModel>> GetSubjectById(int subjectId);
    Task<Response> ToggleSubjectStatus(DtoSubjectStatusToggle request);

    // ================= Role & Menu Management =================
    Task<ResponseList<DtoRoleMasterItem>> GetRoleMasterList();
    Task<Response> SaveRoleMaster(DtoSaveRoleRequest request, long userId);
    Task<ResponseList<DtoMenuListItem>> GetMenuList(string? routePrefix = null);
    Task<ResponseList<DtoParentMenuDropdownItem>> GetParentMenuDropdown(string? routePrefix = null);
    Task<Response> SaveMenu(DtoSaveMenuRequest request, long userId);
    Task<ResponseList<DtoRoleMenuMappingItem>> GetRoleMenuMappingByRoutePrefix(string routePrefix);
    Task<Response> SaveRoleMenuMapping(DtoSaveRoleMenuMappingRequest request, long userId);


    Task<Response> SaveWrittenHallTicketInstruction(WrittenHallTicketInstructionModel model);

    Task<Response> UpdateWrittenHallTicketInstruction( WrittenHallTicketInstructionModel model);

    Task<ResponseList<WrittenHallTicketInstructionModel>> GetAllWrittenHallTicketInstructions();

    Task<Response<WrittenHallTicketInstructionModel>> GetWrittenHallTicketInstructionById(int srNo);

    Task<Response> DeleteWrittenHallTicketInstruction(int srNo);
}