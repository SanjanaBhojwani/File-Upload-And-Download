import { Component,ViewChild,OnInit } from '@angular/core';
import { ApiserviceService } from './Services/apiservice.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { FormGroup,FormBuilder,Validators} from '@angular/forms';
import { HttpClient, HttpResponse } from '@angular/common/http';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(private api: ApiserviceService){}

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort !: MatSort;

  ngOnInit():void{
    this.getAllFiles();
  }
  title = 'uloadFiles';
  fileError: string | null = null;
  selectedFile: File | null = null;

  displayedColumns: string[] = [ 'FileName','action'];
  
  dataSource !: MatTableDataSource<any>;


  onFileSelected(event: any): void {
    const file: File = event.target.files[0];
    if (file) {
      const validTypes = ['image/png', 'image/jpeg', 'image/gif', 'application/pdf'];
      if (validTypes.includes(file.type)) {
        this.selectedFile = file;
        this.fileError = null;
      } else {
        this.fileError = 'Invalid file type. Please select a valid image or PDF file.';
        this.selectedFile = null;
      }
    }
  }

  addFile(): void {
    if (this.selectedFile) {
      this.api.uploadFiles(this.selectedFile)
        .subscribe({
          next: (res) => {
            
            alert('File added successfully');
            this.getAllFiles();
          },
          error: (err) => {
            console.error('Error while adding file:', err);
            alert('Error while adding file');
          }
        });
    } else {
      alert('No file selected');
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  
  onSubmit() {
    
    if (this.selectedFile) {
      console.log('File ready for upload:', this.selectedFile);
      // Perform upload logic here
    }
  }

  
  getAllFiles(){
    this.api.getFiles()
    .subscribe({
      next:(res)=>{
        console.log(res);
        this.dataSource = new MatTableDataSource(res);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
         
      },
      error:()=>{
        alert("error while fetching the records");
      }
    })
  }

  downloadFiles(File_name:string){


  }

  deleteFile(FileID:any){
    this.api.deleteFiles(FileID).subscribe({
      next:(res)=>{
        alert("File Deleted successfully");
        this.getAllFiles();
      },
      error:()=>{
        alert("error while deleting the product")
      }
    })
  }
  // downloadFile(FileName: string) {
  //   // this.isLoading = true;
  //   this.api.downloadFile(FileName).subscribe({
  //     next: (res) => {
  //       setTimeout(() => {
  //         // this.isLoading = false;
  
  //         // Create a URL for the blob and trigger download
  //         const url = window.URL.createObjectURL(res);
  //         const a = document.createElement('a');
  //         a.href = url;
  //         a.download = FileName; // The filename you want to save as
  //         document.body.appendChild(a);
  //         a.click();
  //         window.URL.revokeObjectURL(url);
  
  //         alert("The file is downloaded successfully");
  //       }, 1000);
  //     },
  //     error: (err) => {
  //       // this.isLoading = false;
  //       console.error("Error while downloading the file", err);
  //       alert("Error while downloading the file");
  //     }
  //   });
  // }

  // downloadFile(FileID: any): void {
  //   this.api.downloadFile(FileID).subscribe({
  //     next: (blob) => {
  //       const url = window.URL.createObjectURL(blob);
  //       const a = document.createElement('a');
  //       a.href = url;
  //       a.download = FileID;
  //       a.click();
  //       window.URL.revokeObjectURL(url); // Clean up the URL object
  //     },
  //     error: (err) => {
  //       console.error('Error downloading file:', err);
  //     }
  //   });
  // }
  

  //   downloadFile(FileID: string): void {
  //     this.api.downloadFile(FileID).subscribe({
  
  //     next: (blob: Blob) => {
  //       const downloadUrl = URL.createObjectURL(blob);

  //       const a = document.createElement('a');
  //       a.href = downloadUrl;
  //       a.download = 'downloadedFileName'; // Replace with your desired file name
  //       document.body.appendChild(a);
  //       a.click();
  //       document.body.removeChild(a);
  //       URL.revokeObjectURL(downloadUrl);
  //     },
  //     error: (error: any) => {
  //       console.error('File download failed', error);
  //     },
  //     complete: () => {
  //       console.log('File download completed');
  //     }
  //   });
    
  // }

  // downloadFile(FileName: string) {
  //   // this.isLoading = true;
  //   this.api.downloadFile(FileName).subscribe({
  //     next: (res) => {
  //       setTimeout(() => {
  //         // this.isLoading = false;
   
  //         // Create a URL for the blob and trigger download
  //         const url = window.URL.createObjectURL(res);
  //         const a = document.createElement('a');
  //         a.href = url;
  //         a.download = FileName; // The filename you want to save as
  //         document.body.appendChild(a);
  //         a.click();
  //         window.URL.revokeObjectURL(url);
   
  //         alert("The file is downloaded successfully");
  //       }, 1000);
  //     },
  //     error: (err) => {
  //       // this.isLoading = false;
  //       console.error("Error while downloading the file", err);
  //       alert("Error while downloading the file");
  //     }
  //   });
  // }
 

  downloadFile(FileName: string): void {
    this.api.downloadFile(FileName)
    .subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = FileName;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
      },
      error: (err) => {
        alert("Error while downloading the file");
        console.error('Download Error: ', err);
      }
    })
  }
  }



