import { ExecussionSessionView } from "./ExecussionSessionView";
import { useExecutionSession } from "./ExecutionSessionProvider";

export function ExecContainer() {
    const { sessions } = useExecutionSession()!;

    return (
        <>
            {sessions && <ExecussionSessionView />}
        </>
    );
}