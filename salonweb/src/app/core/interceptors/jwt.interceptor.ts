import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {

  if (req.url.includes('/auth')) {
    return next(req);
  }

  const token = localStorage.getItem('token');
  const authService = inject(AuthService);


  if (token) {

    const cloned = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });

    return next(cloned);
  }

  return next(req).pipe(

    catchError(err => {

      if(err.status === 401){

        return authService.refreshToken().pipe(

          switchMap((res:any)=>{

            localStorage.setItem('accessToken', res.accessToken);

            const newReq = req.clone({
              setHeaders:{
                Authorization:`Bearer ${res.accessToken}`
              }
            });

            return next(newReq);

          })

        );

      }

      return throwError(()=>err);

    })

  );
};