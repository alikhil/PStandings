import * as React from 'react';
import { Link } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';

export class GenerationsComponent extends React.Component<RouteComponentProps<{}>, {}> {
    public render() {
        return <div>
        <h1>Hello, world!</h1>
        <p>Welcome to your new single-page application, built with:</p>
        <Link to={ '/GR1' }> GR 1</Link>
        <Link to={ '/GR2' }> GR 2</Link>
    </div>; 
    }
}
