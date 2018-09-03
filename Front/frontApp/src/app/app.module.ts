import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppConfig } from './app.config';
import { TcpService } from './services/tcp.service';
import { FormsModule } from '@angular/forms';


import {
  MatCardModule,
  MatButtonModule,
  MatChipsModule,
  MatToolbarModule,
  MatGridListModule,
  MatDialogModule,
  MatTabsModule,
  MatTableModule,
  MatTabHeader,
  MatIconModule,
  MatFormFieldModule,
  MatInputModule,
  MatSnackBarModule,
  MatTreeModule,
  MatProgressBarModule,
  MatBadgeModule,
  MatSidenavModule

} from '@angular/material';
import { inspect } from 'util';

@NgModule({
  exports: [
    MatCardModule,
    MatGridListModule,
    MatButtonModule,
    MatChipsModule,
    MatToolbarModule,
    MatDialogModule,
    MatTabsModule,
    MatTableModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSnackBarModule,
    MatTreeModule,
    MatProgressBarModule,
    MatBadgeModule,
    MatSidenavModule

  ],
  declarations: []
})
export class MaterialModule { }

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    MaterialModule,
    FormsModule
  ],
  providers: [
    AppConfig,
    TcpService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
