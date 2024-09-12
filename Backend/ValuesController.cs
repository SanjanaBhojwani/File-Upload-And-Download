using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Net.Http.Headers;
using System.Linq;
using System.Net;
using System.Net.Http;
using DAL;

using System.Web.UI.WebControls;
using System.Web;
using System.Web.Http;
using System.Configuration;
using fileUpload.Models;

namespace fileUpload.Controllers
{
    public class ValuesController : ApiController
    {
        string conn = System.Configuration.ConfigurationManager.ConnectionStrings["DB_connectionstring"].ConnectionString;
        [HttpPost]
        [Route("Main/fileUploadJI")]
        public System.Net.Http.HttpResponseMessage SaveFile(string FileName, string FilePath)
        {

            try
            {
                string save = "UploadFileSB";
                DataSet ds = new DataSet();
                SqlParameter[] paramlist;
                paramlist = new SqlParameter[2];
                paramlist[0] = new SqlParameter("@Filename", FileName);
                paramlist[1] = new SqlParameter("@FilePath", FilePath);
                ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, save, paramlist);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No data Found");
                }

            }
            catch (Exception ex)
            {
                string strMessage;
                strMessage = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, strMessage);
            }
        }


    

    [HttpPost]
    [Route("Main/uploadfile")]
    public string uploading()
    {
        string fname = string.Empty;
        foreach (string file in HttpContext.Current.Request.Files)
        {
            var postedFile = HttpContext.Current.Request.Files[file];
            string strFilePath = string.Empty;  
            string strSaveFilePath = string.Empty;
            string strOrgnFileName = postedFile.FileName;
            strFilePath = System.Web.HttpContext.Current.Server.MapPath("~/uploadFile");

            fname = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/uploadFile/"), Convert.ToString(postedFile.FileName));
                var fileName = Path.GetFileName(postedFile.FileName);
                var filePath = Path.Combine(strFilePath, fileName);
                if (File.Exists(fname))
            {
                strSaveFilePath = strFilePath + strOrgnFileName;
                Console.WriteLine("File uploaded");
                    SaveFile(fileName, filePath);
                }
            else
            {
                Console.WriteLine("File not uploaded");
            }

            strSaveFilePath = strFilePath + strOrgnFileName;
            postedFile.SaveAs(fname);
            return "ok";
        }

        return "error";
    }



        [HttpGet]
        [Route("Main/GetAllData")]
        public IEnumerable<FilesSB> GetAllData()
        {
            var files = new List<FilesSB>();

            using (SqlConnection con = new SqlConnection(conn))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("getAllData", con);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset, "FilesSB");

                foreach (DataRow row in dataset.Tables["FilesSB"].Rows)
                {
                    var file = new FilesSB
                    {
                        FileID = Convert.ToInt32(row["FileID"]),
                        FileName = row["FileName"].ToString(),
                        FilePath = row["FilePath"].ToString(),
                    };
                    files.Add(file);

                }

            }
            return files;
        }

        [HttpDelete]
        [Route("Main/deleteFile/{FileID}")]
        public System.Net.Http.HttpResponseMessage deleteFile(string FileID)
        {
            try
            {
                string sProcName = "deleteFileSB";
                DataSet ds = new DataSet();
                SqlParameter[] paramList;
                paramList = new SqlParameter[1];
                paramList[0] = new SqlParameter("@FileID", FileID);


                ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, sProcName, paramList);
             
                return Request.CreateResponse(HttpStatusCode.OK, ds);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, ex);
            }
        }

        //[HttpGet]
        //[Route("Main/downloadfile")]
        //public System.Net.Http.HttpResponseMessage DownloadFile(string FileName)
        //{
        //    string storedProcedure = "downloadfileR";
        //    DataSet ds = new DataSet();

        //    // Prepare the parameter for the stored procedure
        //    SqlParameter[] paramlist = new SqlParameter[1];
        //    paramlist[0] = new SqlParameter("@FileName", FileName);

        //    // Execute the stored procedure and get the result
        //    ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, storedProcedure, paramlist);

        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        // Extract filename and filepath from the result
        //        string fileName = ds.Tables[0].Rows[0]["FileName"].ToString();
        //        string filePath = ds.Tables[0].Rows[0]["FilePath"].ToString();

        //        if (File.Exists(filePath))
        //        {
        //            // Determine the MIME type based on the file extension
        //            string mimeType = MimeMapping.GetMimeMapping(fileName);
        //            byte[] fileData = File.ReadAllBytes(filePath);

        //            // Prepare the HTTP response with the file data
        //            var response = new HttpResponseMessage(HttpStatusCode.OK)
        //            {
        //                Content = new ByteArrayContent(fileData)
        //            };

        //            // Set content headers for file download
        //            response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //            {
        //                FileName = fileName
        //            };
        //            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);

        //            return response;
        //        }
        //        else
        //        {
        //            // Return a 404 if the file does not exist
        //            return Request.CreateResponse(HttpStatusCode.NotFound, "File not found on the server.");
        //        }
        //    }
        //    else
        //    {
        //        // Return a 404 if no file metadata is found
        //        return Request.CreateResponse(HttpStatusCode.NotFound, "No file metadata found in the database.");
        //    }
        //}

        private readonly string _fileStoragePath = @"C:\Users\S2307508\source\repos\fileUpload\uploadFile"; // Update this to your file storage path

        [HttpGet]
        [Route("Main/download/{fileName}")]
        public HttpResponseMessage DownloadFile(string fileName)
        {
            var filePath = Path.Combine(_fileStoragePath, fileName);

            if (!File.Exists(filePath))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
            }

            var fileBytes = File.ReadAllBytes(filePath);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(fileBytes)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return response;
        }


        [HttpGet]
        [Route("Main/DownloadFile")]
        public HttpResponseMessage DownloadFile3(string FileName)
        {
            try
            {
                string FilePath = null;

                using (SqlConnection con = new SqlConnection(conn))
                {
                    SqlCommand cmd = new SqlCommand("downloadFileSB", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FileName", FileName);

                    con.Open();
                    var result = cmd.ExecuteScalar();
                    FilePath = result != null ? result.ToString() : null;
                }

                if (string.IsNullOrEmpty(FilePath))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File Not Found");
                }

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                response.Content = new StreamContent(fileStream);
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = FileName
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        //[HttpGet]
        //[Route("Main/download/{FileID}")]
        //public HttpResponseMessage DownloadFile(string FileID)
        //{
        //    // Define the path to the file

        //    string filePath = System.Web.HttpContext.Current.Server.MapPath("~/uploadFile"+fileName);

        //    try
        //    {
        //        string sProcName = "downloadfile";
        //        DataSet ds = new DataSet();
        //        SqlParameter[] paramList;
        //        paramList = new SqlParameter[1];
        //        paramList[0] = new SqlParameter("@FileID", FileID);


        //        ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, sProcName, paramList);


        //        // Check if the file exists
        //        if (!File.Exists(filePath))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound, "File not found");
        //        }

        //        // Create the response message
        //        var response = new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        };

        //        // Set the content type and attachment filename
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = fileName
        //        };

        //        return Request.CreateResponse(HttpStatusCode.OK, ds);
        //    }
        //    catch (Exception ex)
        //    {
        //        string msg = ex.Message;
        //        return Request.CreateResponse(HttpStatusCode.OK, ex);
        //    }


        //}

        //[HttpGet]
        //[Route("Main/download/{FileID}")]
        //public HttpResponseMessage DownloadFile(string FileID)
        //{
        //    // Define the connection string and stored procedure name
        //    string sProcName = "downloadfile";
        //    DataSet ds = new DataSet();
        //    SqlParameter[] paramList;
        //    paramList = new SqlParameter[1];
        //    paramList[0] = new SqlParameter("@FileID", FileID);

        //    try
        //    {
        //        // Execute the stored procedure and get the result
        //        ds = SqlHelper.ExecuteDataset(conn, CommandType.StoredProcedure, sProcName, paramList);

        //        // Check if the dataset contains any rows
        //        if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound, "File not found");
        //        }

        //        // Retrieve the file name from the dataset
        //        string fileName = ds.Tables[0].Rows[0]["FileName"].ToString();
        //        string filePath = System.Web.HttpContext.Current.Server.MapPath("~/uploadFile/" + fileName);

        //        // Check if the file exists
        //        if (!File.Exists(filePath))
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound, "File not found");
        //        }

        //        // Create the response message
        //        var response = new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read))
        //        };

        //        // Set the content type and attachment filename
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = fileName
        //        };

        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}



    }
}