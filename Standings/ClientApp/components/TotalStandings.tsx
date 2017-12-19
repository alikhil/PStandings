import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import * as Sticky from 'react-sticky-table';
import '!css-loader!react-sticky-table/dist/react-sticky-table.css';
import { DropdownButton, MenuItem } from 'react-bootstrap';
import autobind from 'autobind-decorator';

import { ComponentWithNav } from './ComponentWithNav';

interface UserResult {
    name: string;
    totalSolved: number;
    solvedInContests: number[];
}

interface StandingsStats {
    contests: string[];
    results: UserResult[];
    isLoading: boolean;
    group: string;
}

export class TotalStandings extends React.Component<RouteComponentProps<{ generation: string}>, StandingsStats> {
    
    constructor(props: RouteComponentProps<{generation: string}>) {
        super(props);
        this.state = { isLoading: true, contests: [], results: [], group: '' };
        fetch(`api/pcms/${props.match.params.generation}/standings`)
            .then(response => response.json() as Promise<StandingsStats>)
            .then(data => this.setState.bind(this)({...data, isLoading: false} ));
    }
    
    public render() {
        const rows:JSX.Element[] = [];
        let columns:JSX.Element[] = [];
        const allGroups:string[] = [];
        const groups: JSX.Element[] = [];
        let id = 1;
        if (this.state.isLoading)
        {
            return  <ComponentWithNav generation={this.props.match.params.generation}>
                <b>Loading...</b>
             </ComponentWithNav>
        }
        ['#', 'Ф.И.О', 'Сумма']
            .concat(this.state.contests)
            .forEach((h, i) => columns.push(<Sticky.Cell key={i} className='tc bg-success'>{h}</Sticky.Cell>));
        rows.push(<Sticky.Row key={0}>{columns}</Sticky.Row>);
        
        this.state.results.forEach((userResult, i) => {
            if (userResult.name.startsWith(this.state.group)) {
                columns = [];
                [id, userResult.name, userResult.totalSolved]
                    .concat(userResult.solvedInContests)
                    .forEach((s, i) => columns.push(<Sticky.Cell key={i} className={ i !== 1 ? 'tc' : ''}>{s}</Sticky.Cell>));
                rows.push(<Sticky.Row key={id}>{columns}</Sticky.Row>);
                id ++;
            }

            const groupName = userResult.name.split(" ")[0];
            if (allGroups.findIndex(v => v === groupName) < 0) {
                allGroups.push(groupName);
            }
        });

        allGroups.sort();
        allGroups.forEach((elem, i) => 
            groups.push(<MenuItem key={i} eventKey={elem} onSelect={this.onGroupSelect}>Группа {elem.slice(2)}</MenuItem>));
        return <div className='w100'>
            <ComponentWithNav generation={this.props.match.params.generation}>
                <DropdownButton title="Группа" id="bg-vertical-dropdown-3" className='mb-10' >
                    <MenuItem key={0} eventKey="all" onSelect={this.onGroupSelect}>Все</MenuItem>
                    { groups }
                </DropdownButton>
                <Sticky.StickyTable stickyColumnCount={3} className={'table-striped h95'}>
                    {rows}
                </Sticky.StickyTable>
            </ComponentWithNav>
        </div>;
    }

    @autobind
    private onGroupSelect(eventKey: any) {
        const groupName = eventKey as string;
        this.setState({...this.state, group: groupName});
    }
}
