import 'zone.js';
import * as Sentry from '@sentry/angular';
import { platformBrowser } from '@angular/platform-browser';

import { AppModule } from './app/app-module';
import { environment } from './environments/environment';

Sentry.init({
  dsn: environment.sentryDsn,
  environment: environment.sentryEnvironment,
  enabled: !!environment.sentryDsn,

  // Keep phase 1 simple: error monitoring only
  tracesSampleRate: 0,

  beforeSend(event, hint) {
    const originalException = hint.originalException;

    // Ignore common expected HTTP/business cases if they bubble here
    // We mostly care about real frontend runtime failures
    return event;
  },
});

platformBrowser()
  .bootstrapModule(AppModule)
  .catch((err) => {
    Sentry.captureException(err);
    console.error(err);
  });
