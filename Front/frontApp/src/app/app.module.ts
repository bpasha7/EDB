import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppConfig } from './app.config';
import { TcpService } from './services/tcp.service';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { DashboardComponent } from './components/dashboard/dashboard.component';
/*Routes */
const appRoutes: Routes = [
  // { path: 'settings', component: SettingsComponent },
  // { path: 'client/:id', component: ClientComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: '', component: DashboardComponent },

];
/*Routes */

import {
  MatCardModule,
  MatButtonModule,
  MatChipsModule,
  MatToolbarModule,
  MatGridListModule,
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
  MatSidenavModule,
  MatListModule,

} from '@angular/material';
import { ViewTableComponent } from './components/view-table/view-table.component';

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
    MatSidenavModule,
    MatListModule

  ],
  declarations: [ViewTableComponent]
})
export class MaterialModule { }

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent
  ],
  imports: [
    RouterModule.forRoot(
      appRoutes
    ),
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
