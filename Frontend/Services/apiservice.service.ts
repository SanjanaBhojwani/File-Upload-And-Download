
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class ApiserviceService {
  private apiUrl = 'http://localhost:44360/Main/'; // Replace with your local API URL

  constructor(private http: HttpClient) { }
  getFiles(): Observable<any> {
    return this.http.get(`${this.apiUrl}GetAllData`);
  }
  uploadFiles(file: File): Observable<any> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post(`${this.apiUrl}uploadfile`, formData);
  }
  deleteFiles(FileID:number): Observable<any>{

    return this.http.delete(`${this.apiUrl}deleteFile/${FileID}`);
    console.log("file deleted")

  }
  // downloadFile(FileName: string): Observable<Blob> {
  //   // Ensure there is no double slash in the URL
  //   return this.http.get<Blob>(`${this.apiUrl}downloadfile/FileName=${encodeURIComponent(FileName)}`, {
  //     responseType: 'blob' as 'json', // Specify response type as blob
  //     withCredentials: true
  //   });
  // }
  // downloadFile(FileID: any): Observable<Blob> {
  //   return this.http.get(`${this.apiUrl}files/download/${FileID}`, { responseType: 'blob' });
  // }

  // downloadFile(fileID: string): Observable<Blob> {
  //   const url = `${this.apiUrl}/download/${fileID}`;
  //   return this.http.get(url, {
  //     responseType: 'blob'  
  //     // Explicitly specify the response type as 'blob'
  //   }) as Observable<Blob>;
  // }

  // downloadFile(FileName: string): Observable<Blob> {
  //   // Ensure there is no double slash in the URL
  //   return this.http.get<Blob>(`${this.apiUrl}/download?FileName=${encodeURIComponent(FileName)}`, {
  //     responseType: 'blob' as 'json', // Specify response type as blob
  //     withCredentials: true
  //   });

  downloadFile(FileName: string): Observable<Blob> {
    const url = `${this.apiUrl}DownloadFile?FileName=${FileName}`;
    return this.http.get(url, {responseType: 'blob'});
  }
  
  


}
