using Profile.Domain.Applicant;

namespace Profile.Application.Applicant
{
    public static class OtherDetailsValidator
    {
        public static void ClearHiddenFields(OtherDetailsRequest d, bool isFemale)
        {
            if (IsOpenCaste(d))
            {
                d.CasteCertificateNo = null;
                d.CasteIssuingDistrict = null;
                d.CasteIssuingDate = null;
                d.IsCasteValidityCertificate = null;
                d.CasteValidityCertificateNo = null;
                d.CasteValidityIssuingDate = null;
                d.IsNonCreamyLayer = null;
                d.NonCreamyLayerCertificateNo = null;
                d.NonCreamyLayerIssuingDate = null;
            }
            else
            {
                if (d.IsCasteValidityCertificate != true)
                {
                    d.CasteValidityCertificateNo = null;
                    d.CasteValidityIssuingDate = null;
                }

                if (d.IsNonCreamyLayer != true)
                {
                    d.NonCreamyLayerCertificateNo = null;
                    d.NonCreamyLayerIssuingDate = null;
                }
            }

            if (d.IsDomicile != true)
            {
                d.DomicileState = null;
                d.DomicileIssuingDistrict = null;
                d.DomicileCertificateNo = null;
                d.DomicileIssuingDate = null;
            }

            if (d.IsPersonWithDisability != true)
            {
                d.DisabilityCategory = null;
                d.DisabilitySubCategory = null;
                d.DisabilityPercentage = null;
                d.DisabilityCertificateType = null;
                d.DisabilityCertificateNo = null;
                d.DisabilityIssuingDate = null;
                d.IsUDIDCard = null;
                d.UDIDNo = null;
            }
            else if (d.IsUDIDCard != true)
            {
                d.UDIDNo = null;
            }

            if (d.IsSportsPerson != true)
            {
                d.SportsCertificateNo = null;
                d.SportsIssuingDate = null;
            }

            if (d.IsExServiceman == true)
            {
                d.IsExServicemanDependent = null;
                d.ExServicemanDependentCertificateNo = null;
                d.ExServicemanDependentIssuingDate = null;
            }
            else
            {
                d.ExServicemanCertificateNo = null;
                d.ExServicemanIssuingDate = null;
                if (d.IsExServicemanDependent != true)
                {
                    d.ExServicemanDependentCertificateNo = null;
                    d.ExServicemanDependentIssuingDate = null;
                }
            }

            if (d.IsMaharashtraEmployee != true)
                d.SevaarthID = null;

            if (d.IsAdditionalReservation != true)
            {
                d.IsProjectAffected = null;
                d.ProjectAffectedCertificateNo = null;
                d.ProjectAffectedIssuingDate = null;
                d.IsEarthquakeAffected = null;
                d.EarthquakeAffectedCertificateNo = null;
                d.EarthquakeAffectedIssuingDate = null;
                d.IsAnshakalin = null;
                d.AnshakalinCertificateNo = null;
                d.AnshakalinIssuingDate = null;
                d.IsPoliceChild = null;
                d.PoliceChildCertificateNo = null;
                d.PoliceChildIssuingDate = null;
                d.IsHomeGuard = null;
                d.HomeGuardCertificateNo = null;
                d.HomeGuardIssuingDate = null;
                d.IsWomanReservation = null;
                d.WomanReservationCertificateNo = null;
                d.WomanReservationIssuingDate = null;
            }
            else
            {
                if (d.IsProjectAffected != true)
                {
                    d.ProjectAffectedCertificateNo = null;
                    d.ProjectAffectedIssuingDate = null;
                }
                if (d.IsEarthquakeAffected != true)
                {
                    d.EarthquakeAffectedCertificateNo = null;
                    d.EarthquakeAffectedIssuingDate = null;
                }
                if (d.IsAnshakalin != true)
                {
                    d.AnshakalinCertificateNo = null;
                    d.AnshakalinIssuingDate = null;
                }
                if (d.IsPoliceChild != true)
                {
                    d.PoliceChildCertificateNo = null;
                    d.PoliceChildIssuingDate = null;
                }
                if (d.IsHomeGuard != true)
                {
                    d.HomeGuardCertificateNo = null;
                    d.HomeGuardIssuingDate = null;
                }
                if (d.IsWomanReservation != true)
                {
                    d.WomanReservationCertificateNo = null;
                    d.WomanReservationIssuingDate = null;
                }
            }

            if (!isFemale)
            {
                d.IsWomanReservation = false;
                d.WomanReservationCertificateNo = null;
                d.WomanReservationIssuingDate = null;
            }

            if (d.IsOrphanReservation != true)
            {
                d.OrphanType = null;
                d.OrphanCertificateNo = null;
                d.OrphanCertificateIssuingDate = null;
            }
        }

        public static Dictionary<string, string[]> ValidateForSave(OtherDetailsRequest d, bool isFemale)
        {
            var errors = new Dictionary<string, string[]>();
            Require(errors, d.Height.HasValue, nameof(d.Height), "Height is required.");
            Require(errors, d.Weight.HasValue, nameof(d.Weight), "Weight is required.");
            Require(errors, d.ChestDeflated.HasValue, nameof(d.ChestDeflated), "Chest deflated is required.");
            Require(errors, d.ChestInflated.HasValue, nameof(d.ChestInflated), "Chest inflated is required.");
            Require(errors, d.CasteCategory.HasValue, nameof(d.CasteCategory), "Caste category is required.");

            if (!IsOpenCaste(d))
            {
                Require(errors, HasText(d.CasteCertificateNo), nameof(d.CasteCertificateNo), "Caste certificate number is required.");
                Require(errors, d.CasteIssuingDistrict.HasValue, nameof(d.CasteIssuingDistrict), "Caste issuing district is required.");
                Require(errors, d.CasteIssuingDate.HasValue, nameof(d.CasteIssuingDate), "Caste issuing date is required.");

                if (d.IsCasteValidityCertificate == true)
                {
                    Require(errors, HasText(d.CasteValidityCertificateNo), nameof(d.CasteValidityCertificateNo), "Caste validity certificate number is required.");
                    Require(errors, d.CasteValidityIssuingDate.HasValue, nameof(d.CasteValidityIssuingDate), "Caste validity issuing date is required.");
                }

                if (d.IsNonCreamyLayer == true)
                {
                    Require(errors, HasText(d.NonCreamyLayerCertificateNo), nameof(d.NonCreamyLayerCertificateNo), "NCL certificate number is required.");
                    Require(errors, d.NonCreamyLayerIssuingDate.HasValue, nameof(d.NonCreamyLayerIssuingDate), "NCL issuing date is required.");
                }
            }

            Require(errors, d.IsDomicile == true, nameof(d.IsDomicile), "Domicile certificate is mandatory. Please select Yes.");
            if (d.IsDomicile == true)
            {
                Require(errors, d.DomicileState.HasValue, nameof(d.DomicileState), "Domicile state is required.");
                Require(errors, HasText(d.DomicileCertificateNo), nameof(d.DomicileCertificateNo), "Domicile certificate number is required.");
                Require(errors, d.DomicileIssuingDistrict.HasValue, nameof(d.DomicileIssuingDistrict), "Domicile issuing district is required.");
                Require(errors, d.DomicileIssuingDate.HasValue, nameof(d.DomicileIssuingDate), "Domicile issuing date is required.");
            }

            Require(errors, d.IsPersonWithDisability.HasValue, nameof(d.IsPersonWithDisability), "Person with disability is required.");
            if (d.IsPersonWithDisability == true)
            {
                Require(errors, d.DisabilityCategory.HasValue, nameof(d.DisabilityCategory), "Disability category is required.");
                Require(errors, d.DisabilitySubCategory.HasValue, nameof(d.DisabilitySubCategory), "Disability sub category is required.");
                Require(errors, d.DisabilityPercentage.HasValue, nameof(d.DisabilityPercentage), "Disability percentage is required.");
                if (d.DisabilityPercentage.HasValue && (d.DisabilityPercentage.Value <= 0 || d.DisabilityPercentage.Value > 100))
                {
                    errors[nameof(d.DisabilityPercentage)] = new[] { "Disability percentage must be between 1 and 100." };
                }
                Require(errors, d.DisabilityCertificateType.HasValue, nameof(d.DisabilityCertificateType), "Disability certificate type is required.");
                Require(errors, HasText(d.DisabilityCertificateNo), nameof(d.DisabilityCertificateNo), "Disability certificate number is required.");
                Require(errors, d.DisabilityIssuingDate.HasValue, nameof(d.DisabilityIssuingDate), "Disability issuing date is required.");
                Require(errors, d.IsUDIDCard.HasValue, nameof(d.IsUDIDCard), "UDID card is required.");
                if (d.IsUDIDCard == true)
                    Require(errors, HasText(d.UDIDNo), nameof(d.UDIDNo), "UDID number is required.");
            }

            Require(errors, d.IsSportsPerson.HasValue, nameof(d.IsSportsPerson), "Sports person is required.");
            if (d.IsSportsPerson == true)
            {
                Require(errors, HasText(d.SportsCertificateNo), nameof(d.SportsCertificateNo), "Sports certificate number is required.");
                Require(errors, d.SportsIssuingDate.HasValue, nameof(d.SportsIssuingDate), "Sports issuing date is required.");
            }

            Require(errors, d.IsExServiceman.HasValue, nameof(d.IsExServiceman), "Ex-serviceman is required.");
            if (d.IsExServiceman == true)
            {
                Require(errors, HasText(d.ExServicemanCertificateNo), nameof(d.ExServicemanCertificateNo), "Ex-serviceman certificate number is required.");
                Require(errors, d.ExServicemanIssuingDate.HasValue, nameof(d.ExServicemanIssuingDate), "Ex-serviceman issuing date is required.");
            }
            else if (d.IsExServiceman == false)
            {
                Require(errors, d.IsExServicemanDependent.HasValue, nameof(d.IsExServicemanDependent), "Ex-serviceman dependent is required.");
                if (d.IsExServicemanDependent == true)
                {
                    Require(errors, HasText(d.ExServicemanDependentCertificateNo), nameof(d.ExServicemanDependentCertificateNo), "Dependent certificate number is required.");
                    Require(errors, d.ExServicemanDependentIssuingDate.HasValue, nameof(d.ExServicemanDependentIssuingDate), "Dependent issuing date is required.");
                }
            }

            Require(errors, d.IsMaharashtraEmployee.HasValue, nameof(d.IsMaharashtraEmployee), "Maharashtra employee is required.");
            if (d.IsMaharashtraEmployee == true)
                Require(errors, HasText(d.SevaarthID), nameof(d.SevaarthID), "Sevaarth ID is required.");

            Require(errors, d.IsAdditionalReservation.HasValue, nameof(d.IsAdditionalReservation), "Additional reservation is required.");
            if (d.IsAdditionalReservation == true)
            {
                Require(errors, d.IsProjectAffected.HasValue, nameof(d.IsProjectAffected), "Project affected is required.");
                if (d.IsProjectAffected == true)
                {
                    Require(errors, HasText(d.ProjectAffectedCertificateNo), nameof(d.ProjectAffectedCertificateNo), "Project affected certificate number is required.");
                    Require(errors, d.ProjectAffectedIssuingDate.HasValue, nameof(d.ProjectAffectedIssuingDate), "Project affected issuing date is required.");
                }

                Require(errors, d.IsEarthquakeAffected.HasValue, nameof(d.IsEarthquakeAffected), "Earthquake affected is required.");
                if (d.IsEarthquakeAffected == true)
                {
                    Require(errors, HasText(d.EarthquakeAffectedCertificateNo), nameof(d.EarthquakeAffectedCertificateNo), "Earthquake affected certificate number is required.");
                    Require(errors, d.EarthquakeAffectedIssuingDate.HasValue, nameof(d.EarthquakeAffectedIssuingDate), "Earthquake affected issuing date is required.");
                }

                Require(errors, d.IsAnshakalin.HasValue, nameof(d.IsAnshakalin), "Anshakalin is required.");
                if (d.IsAnshakalin == true)
                {
                    Require(errors, HasText(d.AnshakalinCertificateNo), nameof(d.AnshakalinCertificateNo), "Anshakalin certificate number is required.");
                    Require(errors, d.AnshakalinIssuingDate.HasValue, nameof(d.AnshakalinIssuingDate), "Anshakalin issuing date is required.");
                }

                Require(errors, d.IsPoliceChild.HasValue, nameof(d.IsPoliceChild), "Police child is required.");
                if (d.IsPoliceChild == true)
                {
                    Require(errors, HasText(d.PoliceChildCertificateNo), nameof(d.PoliceChildCertificateNo), "Police child certificate number is required.");
                    Require(errors, d.PoliceChildIssuingDate.HasValue, nameof(d.PoliceChildIssuingDate), "Police child issuing date is required.");
                }

                Require(errors, d.IsHomeGuard.HasValue, nameof(d.IsHomeGuard), "Homeguard is required.");
                if (d.IsHomeGuard == true)
                {
                    Require(errors, HasText(d.HomeGuardCertificateNo), nameof(d.HomeGuardCertificateNo), "Homeguard certificate number is required.");
                    Require(errors, d.HomeGuardIssuingDate.HasValue, nameof(d.HomeGuardIssuingDate), "Homeguard issuing date is required.");
                }

                if (isFemale)
                {
                    Require(errors, d.IsWomanReservation.HasValue, nameof(d.IsWomanReservation), "Woman reservation is required.");
                    if (d.IsWomanReservation == true)
                    {
                        Require(errors, HasText(d.WomanReservationCertificateNo), nameof(d.WomanReservationCertificateNo), "Woman reservation certificate number is required.");
                        Require(errors, d.WomanReservationIssuingDate.HasValue, nameof(d.WomanReservationIssuingDate), "Woman reservation issuing date is required.");
                    }
                }
            }

            Require(errors, d.IsOrphanReservation.HasValue, nameof(d.IsOrphanReservation), "Orphan reservation is required.");
            if (d.IsOrphanReservation == true)
            {
                Require(errors, d.OrphanType.HasValue, nameof(d.OrphanType), "Orphan type is required.");
                Require(errors, HasText(d.OrphanCertificateNo), nameof(d.OrphanCertificateNo), "Orphan certificate number is required.");
                Require(errors, d.OrphanCertificateIssuingDate.HasValue, nameof(d.OrphanCertificateIssuingDate), "Orphan issuing date is required.");
            }

            ValidatePhysicalMeasurement(d.Height, nameof(d.Height), "Height", errors);
            ValidatePhysicalMeasurement(d.Weight, nameof(d.Weight), "Weight", errors);
            ValidatePhysicalMeasurement(d.ChestDeflated, nameof(d.ChestDeflated), "Chest deflated", errors);
            ValidatePhysicalMeasurement(d.ChestInflated, nameof(d.ChestInflated), "Chest inflated", errors);

            ValidateCertificateNo(d.CasteCertificateNo, nameof(d.CasteCertificateNo), "Caste certificate number", errors);
            ValidateCertificateNo(d.CasteValidityCertificateNo, nameof(d.CasteValidityCertificateNo), "Caste validity certificate number", errors);
            ValidateCertificateNo(d.NonCreamyLayerCertificateNo, nameof(d.NonCreamyLayerCertificateNo), "Non-creamy layer certificate number", errors);
            ValidateCertificateNo(d.DomicileCertificateNo, nameof(d.DomicileCertificateNo), "Domicile certificate number", errors);
            ValidateCertificateNo(d.DisabilityCertificateNo, nameof(d.DisabilityCertificateNo), "Disability certificate number", errors);
            ValidateCertificateNo(d.UDIDNo, nameof(d.UDIDNo), "UDID number", errors);
            ValidateCertificateNo(d.SportsCertificateNo, nameof(d.SportsCertificateNo), "Sports certificate number", errors);
            ValidateCertificateNo(d.ExServicemanCertificateNo, nameof(d.ExServicemanCertificateNo), "Ex-serviceman certificate number", errors);
            ValidateCertificateNo(d.ExServicemanDependentCertificateNo, nameof(d.ExServicemanDependentCertificateNo), "Ex-serviceman dependent certificate number", errors);
            ValidateCertificateNo(d.SevaarthID, nameof(d.SevaarthID), "Sevaarth ID", errors);
            ValidateCertificateNo(d.ProjectAffectedCertificateNo, nameof(d.ProjectAffectedCertificateNo), "Project affected certificate number", errors);
            ValidateCertificateNo(d.EarthquakeAffectedCertificateNo, nameof(d.EarthquakeAffectedCertificateNo), "Earthquake affected certificate number", errors);
            ValidateCertificateNo(d.AnshakalinCertificateNo, nameof(d.AnshakalinCertificateNo), "Anshakalin certificate number", errors);
            ValidateCertificateNo(d.PoliceChildCertificateNo, nameof(d.PoliceChildCertificateNo), "Police child certificate number", errors);
            ValidateCertificateNo(d.HomeGuardCertificateNo, nameof(d.HomeGuardCertificateNo), "Home guard certificate number", errors);
            ValidateCertificateNo(d.WomanReservationCertificateNo, nameof(d.WomanReservationCertificateNo), "Woman reservation certificate number", errors);
            ValidateCertificateNo(d.OrphanCertificateNo, nameof(d.OrphanCertificateNo), "Orphan certificate number", errors);

            return errors;
        }

        private static void ValidatePhysicalMeasurement(decimal? val, string key, string name, Dictionary<string, string[]> errors)
        {
            if (!val.HasValue) return;
            var str = val.Value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            if (!System.Text.RegularExpressions.Regex.IsMatch(str, @"^\d{1,3}(\.\d{1,2})?$") || val.Value <= 0)
            {
                errors[key] = new[] { $"{name} must allow up to 3 digits before decimal and 2 digits after decimal (e.g., 123.43)." };
            }
        }

        private static void ValidateCertificateNo(string? certNo, string key, string name, Dictionary<string, string[]> errors)
        {
            if (string.IsNullOrWhiteSpace(certNo)) return;
            if (certNo.Length > 25)
            {
                errors[key] = new[] { $"{name} cannot exceed 25 characters." };
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(certNo, @"^[a-zA-Z0-9()./\[\]_-]{1,25}$"))
            {
                errors[key] = new[] { $"{name} contains invalid characters. Allowed characters are alphanumeric and ()./[]-_." };
            }
        }

        private static bool IsOpenCaste(OtherDetailsRequest d)
        {
            if (!d.CasteCategory.HasValue || d.CasteCategory.Value == 0)
            {
                return true;
            }

            return d.CasteCategory.Value == 1;
        }

        private static bool HasText(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        private static void Require(Dictionary<string, string[]> errors, bool ok, string key, string message)
        {
            if (!ok)
                errors[key] = new[] { message };
        }
    }
}
