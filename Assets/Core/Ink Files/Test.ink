EXTERNAL StartQuest(name)

-> Main


=== Main 
Hey there stranger! I am Arthur, the mighty guard of the royal family. I have been sent on a mission to bring justice to innocent people by slaying this beast. Can you help me to defeat this ? 

* [Yes, I will help you.] -> Help
* [Sorry, can't help you now.] ->NoHelp

== Default
Remain silent.
->DONE

== Help
I will be honored to help you my friend.
~ StartQuest("QTest2")
->DONE

== NoHelp
I am in a hurry right now. Sorry, for not being able to help.
->DONE 

== Completion1
You have completed my task.
->DONE

== NonCompletion1
You have failed to complete my task.
->DONE


== function StartQuest(name)
~return 



