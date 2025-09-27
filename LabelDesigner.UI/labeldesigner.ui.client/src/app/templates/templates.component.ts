import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DesignerModalComponent } from '../designer-modal/designer-modal.component';
import { Template } from '../shared/models/template.model';
import { HttpClient } from '@angular/common/http';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { TemplateNew } from '../shared/models/templatenew.modal';
import { ElementRef, HostListener } from '@angular/core';
import { Constants } from '../common/constants.model';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-templates',
  templateUrl: './templates.component.html',
  imports: [CommonModule, RouterModule, DesignerModalComponent, FormsModule],
  styleUrls: ['./templates.component.css']
})
export class TemplatesComponent implements OnInit {
  labels: string[] = [];
  templates: TemplateNew[] = [];
  filteredTemplates: TemplateNew[] = [];
  showDesigner: boolean = false;
  templateId: string;
  templateSvg: string;
  fabricJson: string;
  templateName: string;
  templateVersionId: string;
  isRequestPayload = false;
  requestPayloadData: any;
  template: TemplateNew = new TemplateNew();
  searchText: string = '';

  constructor(private http: HttpClient
    , private sanitizer: DomSanitizer
    , private elementRef: ElementRef) {

  }
  ngOnInit() {
    this.getTemplates();
  }
  openDesigner() {
    this.templateId = '';
    this.templateSvg = '';
    this.fabricJson = '';
    this.templateVersionId = '';
    this.templateName = 'Label Template'
    this.showDesigner = true;
  }

  editDesigner(templateId: string, templateSvg: string, fabricJson: string, templateName: string, templateVersionId: string) {
    this.templateId = templateId;
    this.templateSvg = templateSvg;
    this.fabricJson = fabricJson
    this.templateName = templateName;
    this.templateVersionId = templateVersionId;
    this.template = {
      sysId: templateId,
      templateSvg: templateSvg,
      versionId: templateVersionId,
      name: templateName,
      fabricJson: fabricJson,
      canvasSettings: ""
    };
    this.showDesigner = true;

  }

  // Method to add a new label from "designer"
  addLabelFromDesigner(template?: Template) {
    //this.templates.push(template);

  }

  // Method to add a label manually
  addLabel() {
    this.labels.push(`New Label ${this.labels.length + 1}`);
  }
  closeDesigner() {
    this.showDesigner = false;
  }
  savedTemplate(template: Template) {
    this.openedMenuId = null;
    this.closeDesigner();
    this.getTemplates();
  }

  deleteDesigner(templateId: string) {
    var deleteTemplateUrl = Constants.ApiBaseUrl + Constants.SaveTemplateUrl + '/' + templateId;
    this.http.delete(deleteTemplateUrl).subscribe({
      next: (response) => {
        console.log('Deleted successfully:', response);
        this.getTemplates();
        // Optionally refresh the list or give feedback here
      },
      error: (error) => {
        console.error('Delete failed:', error);
        // Show error notification if you want
      }
    });
  }

  cloneTemplate(templateId: string) {
    var cloneTemplateUrl = Constants.ApiBaseUrl + Constants.CloneTemplateUrl + templateId;
    this.http.post(cloneTemplateUrl, null).subscribe({
      next: (response) => {
        console.log('Cloned successfully:', response);
        this.getTemplates();
        // Optionally refresh the list or give feedback here
      },
      error: (error) => {
        console.error('Cloning failed:', error);
        // Show error notification if you want
      }
    })
  }

  getSafeSvg(svg: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(svg);
  }
  base64EncodeSvg(svg: string): string {
    // safer encoding to handle special chars in SVG
    return 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svg)));
  }
  getTemplates() {
    var getTemplatesUrl = Constants.ApiBaseUrl + Constants.SaveTemplateUrl
    this.http.get(getTemplatesUrl)
      .subscribe(data => {
        console.log('Received data:', data);
        if (Array.isArray(data)) {
          this.templates = data.map(item => new TemplateNew(item));
          this.filteredTemplates = this.templates;
          if(this.searchText){
            this.filterTemplates();
          }
        } else {
          console.error('Data is not an array:', data);
          // Handle single object case or error
          this.templates = [new TemplateNew(data)];
        }
      });
  }

  filterTemplates() {
    const query = this.searchText.trim().toLowerCase();

    if (!query) {
      this.filteredTemplates = this.templates; // reset
      return;
    }

    this.filteredTemplates = this.templates.filter(t =>
      (t.name && t.name.toLowerCase().includes(query))
    );
  }
  
  clearSearch() {
  this.searchText = '';
  this.filteredTemplates = this.templates;
}


  getRequestPayload(templateId) {
    var targetTemplate = this.templates.filter(x => x.sysId == templateId)[0];

    var fabricJson = JSON.parse(targetTemplate.fabricJson);
    var templateObjects = fabricJson.objects;
    if (templateObjects && templateObjects.length > 0) {
      var template = {
        templateId: targetTemplate.sysId,
        templateWidgets: []
      }

      for (var i = 0; i < templateObjects.length; i++) {
        var widgetType = templateObjects[i].type;

        if (widgetType == 'group') {

          var childWidgets = templateObjects[i].objects;
          for (var j = 0; j < childWidgets.length; j++) {
            this._getTemplateWidgetsObject(childWidgets[j], template);
          }
        }
        else {

          this._getTemplateWidgetsObject(templateObjects[i], template);
        }
      }

      const payload = {

        "templateId": `${template.templateId}`,
        "height": 0,
        "width": 0,
        "records": [
          {
            "templateWidgets": template.templateWidgets
          }
        ]
      };
      this.isRequestPayload = true;
      this.requestPayloadData = payload;
    }
    else {
      alert("No widgets available");
    }
  }
  isLoadingPdf: boolean = false;
  submitRequestPayload() {
    console.log("Submitted Payload:", this.requestPayloadData);
    this.isLoadingPdf = true;
    // Call API or handle form data here
    this.http.post(
      Constants.ApiBaseUrl + Constants.GenerateTestPDF,
      this.requestPayloadData,
      { responseType: 'blob' } // <-- important
    ).subscribe({
      next: (response: Blob) => {
        console.log('PDF generated successfully');

        // Create a URL for the blob
        const blob = new Blob([response], { type: 'application/pdf' });
        const url = window.URL.createObjectURL(blob);

        // Open PDF in a new tab
        window.open(url);
        this.isLoadingPdf = false;
        this.isRequestPayload = false;
        // Or, download directly
        // const a = document.createElement('a');
        // a.href = url;
        // a.download = 'Test.pdf';
        // a.click();
        // window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('PDF generation failed:', error);
        this.isLoadingPdf = false;
        this.isRequestPayload = false;
      }
    });
  }

  private _getTemplateWidgetsObject(templateObject: any, template: any): any {

    let widgetType: any = templateObject.CustomType;
    let widgetId: string = templateObject.Id;
    let isDynamic: boolean = templateObject.IsDynamic;

    if ((widgetType === 'textbox' && isDynamic === true) || widgetType === 'barcode') {
      let initialValue = '';

      if (widgetType === 'textbox') {
        initialValue = 'Sample Text ' + Math.floor(Math.random() * 1000);
      } else if (widgetType === 'barcode') {
        initialValue = Math.floor(100000 + Math.random() * 900000).toString(); // 6-digit number
      }

      template.templateWidgets.push({
        widgetId: widgetId,
        widgetValue: initialValue
      });
    }
  }
  openedMenuId: string | null = null;

  toggleMenu(sysId: string): void {
    this.openedMenuId = this.openedMenuId === sysId ? null : sysId;
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const clickedInside = this.elementRef.nativeElement.contains(event.target);
    if (!clickedInside) {
      this.openedMenuId = null;
    }
  }
}
function ngAfterViewChecked() {
  throw new Error('Function not implemented.');
}

