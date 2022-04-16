EXTERNAL startQuest(name)
VAR IsCompleted = false
LIST QuestStates = (Idle), Start, GoalSatisfied, GoalUnsatisfied, Completed, Failed
VAR currentState1 = Idle
-> Main
=== Main 
~currentState1 = Idle
Hey there stranger! I am Arthur, the mighty guard of the royal family. I have been sent on a mission to bring justice to innocent people by slaying this beast. Can you help me to defeat this ? 

* [Yes, I will help you.] -> Help
* [Sorry, can't help you now.] ->NoHelp
- -> Default
- -> Completion
- -> NonCompletion




== Default
Remain silent.
->DONE

== Help
I will be honored to help you my friend.
//~RemoveFromList(Idle)
//~AddToList(QuestStart)
~startQuest("QTest1")


->DONE

== NoHelp
I am in a hurry right now. Sorry, for not being able to help.
->DONE

== Completion
{currentState1 == GoalSatisfied : currentState = Completed} ->PostCompletion
->DONE

== PostCompletion
Thank you for completing the mission.
->END

== NonCompletion
{currentState1 == GoalUnsatisfied : currentState = Failed} ->PostNonCompletion
->DONE

== PostNonCompletion
Please be hurry with the mission.
->END

== function startQuest(name)
~ return

== function AddToList(CS)
    { QuestStates !? CS:
        ~QuestStates += CS
    }
    
== function RemoveFromList(CS)
    { QuestStates ? CS:
        ~QuestStates -= CS
    }

