import autobind from 'autobind-decorator';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { ComponentWithNav } from './ComponentWithNav';
import { DropdownButton, MenuItem } from 'react-bootstrap';


interface ContestState {
    isLoading: boolean;
    contest: Contest;
    group: string;
    contestId: string;
}

interface ContestProbs {
    contestId: string; 
    generation: string;
}

interface Problem {
    name: string;
    alias: string;
}

interface UserResult {
    name: string;
    totalSolved: number;
    problemsStatus: string[];
}

interface Contest {
    problems: Problem[];
    results: UserResult[];
    name: string;
}

function shallowEqual(objA: any, objB: any): boolean {
    // return JSON.stringify(objA) == JSON.stringify(objB);
    if (objA === objB) {
      return true;
    }
  
    if (typeof objA !== 'object' || objA === null ||
        typeof objB !== 'object' || objB === null) {
      return false;
    }
  
    var keysA = Object.keys(objA);
    var keysB = Object.keys(objB);
  
    if (keysA.length !== keysB.length) {
      return false;
    }
  
    // Test for A's keys different from B.
    var bHasOwnProperty = Object.hasOwnProperty.bind(objB);
    for (var i = 0; i < keysA.length; i++) {
      if (!bHasOwnProperty(keysA[i]) || objA[keysA[i]] !== objB[keysA[i]]) {
        return false;
      }
    }
  
    return true;
  }

export class ContestComponent extends React.Component<RouteComponentProps<ContestProbs>, ContestState> {
    constructor() {
        super();
        this.state = { isLoading: true, contest: { problems: [], results: [], name: '' }, group: '', contestId: '' };
    }

    public shouldComponentUpdate(nextProps: RouteComponentProps<ContestProbs>, nextState: ContestState) {
        return (nextProps.location !==  this.props.location || !shallowEqual(nextState, this.state));
    }

    public componentWillReceiveProps() {
        this.setState({...this.state, contestId: this.props.match.params.contestId, isLoading: true}, () => {
            this.getData();
        });
    }
    
    public componentDidMount() {
        this.getData();
    }

    private getData() {
        fetch(`api/pcms/${this.props.match.params.generation}/contest/${this.props.match.params.contestId}`)
            .then(response => response.json() as Promise<Contest>)
            .then(data => this.setState({ ...this.state, isLoading: false, contest: data }));
    }

    @autobind
    private onGroupSelect(eventKey: any) {
        const groupName = eventKey as string;
        this.setState({...this.state, group: groupName});
    }

    public render() {
        if (this.state.isLoading)
        {
            return  <ComponentWithNav generation={this.props.match.params.generation}>
                <b>Loading...</b>
             </ComponentWithNav>
        }
        
        const allGroups = Array.from(new Set(this.state.contest.results.map(r => r.name.split(" ")[0])))
        allGroups.sort();

        const groups: JSX.Element[] = [];
        allGroups.forEach((elem, i) => 
            groups.push(<MenuItem key={i} eventKey={elem} onSelect={this.onGroupSelect}>Группа {elem.slice(2)}</MenuItem>));
        
        const problems = this.state.contest.problems.map((problem, i) =>
            <td key={i}> <span title={ problem.name }> { problem.alias }</span></td>
        );
        const results = this.state.contest.results.filter(r => r.name.startsWith(this.state.group)).map((result, i) => 
            <tr key={i}>
                <td className='tc'> {i + 1}</td>
                <td> {result.name} </td>
                <td className='tc'><b>{result.totalSolved}</b></td>
                { result.problemsStatus.map((status, j) => 
                    <td key={j} className={'tc ' + (status.startsWith('+') ? 'my-success' : (status.startsWith('-') ? 'my-danger' : '')) }>{status}</td>
                ) }
            </tr>
        );

        return <ComponentWithNav generation={this.props.match.params.generation}>
            <h1> { this.state.contest.name } </h1>
            <DropdownButton title="Группа" id="bg-vertical-dropdown-3" className='mb-10' >
                <MenuItem key={0} eventKey="all" onSelect={this.onGroupSelect}>Все</MenuItem>
                { groups }
            </DropdownButton>
            <br/>

            <div  className='panel panel-default'>
                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <td className='tc'>№</td>
                            <td className='tc'>Ф.И.О</td>
                            <td className='tc'><b>Сумма</b></td>
                            { problems }
                        </tr>
                    </thead>
                    <tbody>

                    { results }
                    </tbody>
                </table>
            </div>

        </ComponentWithNav>
    }
}