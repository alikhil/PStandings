import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { ContestComponent } from './components/Contest';
import { TotalStandings } from './components/TotalStandings';
import { SubmissionsComponent } from './components/Submissions';
import { GenerationsComponent } from './components/Generations';
import { ComponentWithNav } from './components/ComponentWithNav';
import { AnalyticsComponent } from './components/Analytics';

export const routes = <Layout>
    <Route exact path='/' component={ GenerationsComponent } />
    <Route exact path='/:generation' component={ TotalStandings } />
    <Route path='/:generation/submissions' component={ SubmissionsComponent } />
    <Route path='/:generation/contests/:contestId' component = { ContestComponent } />
    <Route path='/:generation/analytics' component = { AnalyticsComponent } />
</Layout>;
