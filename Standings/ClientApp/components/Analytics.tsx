import autobind from 'autobind-decorator';
import * as mm from 'moment';
import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';

import { DatePicker } from './DateRangePicker';
import { ComponentWithNav } from './ComponentWithNav';
import { NavMenu } from './NavMenu';
import { Nav, NavItem } from 'react-bootstrap';
import { UserAnalyticsComponent } from './analytics/UserAnalytics';
import { ContestAnalyticsComponent } from './analytics/ContestAnalytics';
import { CommonAnalyticsComponent } from './analytics/CommonAnalytics';

interface AnaliticsState  {
    activeTab: string;
}

export class AnalyticsComponent extends React.Component<RouteComponentProps<{ generation: string }>, AnaliticsState> {
    
    constructor() {
        super();
        this.state = { activeTab: 'main' };
    }

    @autobind
    private handleSelect(key: any) {
        this.setState({...this.state, activeTab: key})
        console.log(`selected ${key}`);
    }

    public render() {
        return <ComponentWithNav generation={this.props.match.params.generation}>
            <Nav bsStyle="tabs" activeKey={this.state.activeTab} onSelect={this.handleSelect}>
                <NavItem eventKey='main' >Общая</NavItem>
                <NavItem eventKey='contests' title="Item">Контесты</NavItem>
                <NavItem eventKey='users' disabled>Участники</NavItem>
            </Nav>
            <br/>
            { this.state.activeTab === 'main' 
                ? <CommonAnalyticsComponent /> 
                : (this.state.activeTab === 'contests' 
                    ? <ContestAnalyticsComponent />
                    : <UserAnalyticsComponent />)
            }
        </ComponentWithNav>;
    }
}