import * as React from 'react';
import { NavMenu } from './NavMenu';
import { RouteComponentProps } from 'react-router-dom';

export class ComponentWithNav extends React.Component<{ generation:string }, {}> {
    public render() {
        return <div>
            <div className='col-sm-2'>
                <NavMenu generation={this.props.generation}/>
            </div>
            <div className='col-sm-10'>
                <br/>
                {this.props.children}
            </div>
        </div>;
    }
}
