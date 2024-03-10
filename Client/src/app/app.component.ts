import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'Dating app';
  users: any;
  constructor(private http: HttpClient) { }
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/Users').subscribe({
      next: (response) => {
        this.users = response

      },
      error: error => console.log(error),
      complete: () => console.log('request has completed')
    })
    }
}
