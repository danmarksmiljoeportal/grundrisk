import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

// Demo
const ApiUrl = 'https://grundrisk-api.demo.miljoeportal.dk/';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent {
    public bodyRequest: string;
    public isSubmitting = false;
    public results: any;

    constructor(private readonly http: HttpClient) {

    }

    public submit(): void {
        if (!this.bodyRequest) {
            return;
        }

        this.isSubmitting = true;

        try {
            const url = ApiUrl + 'screenings/preliminary';
            const body = JSON.parse(this.bodyRequest);

            this.http.post<any>(url, body).subscribe(
                (res) => {
                    this.results = res;
                    this.isSubmitting = false;
                },
                (err) => {
                    this.results = err;
                    this.isSubmitting = false;
                }
            );
        } catch (ex) {
            this.results = ex.toString();
            this.isSubmitting = false;
        }
    }
}
